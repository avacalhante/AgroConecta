import * as FileSystem from 'expo-file-system/legacy';

import { enviarEvidencia } from '../api/entregas';
import { ApiError, erroDefinitivo } from '../api/http';
import type { Entrega } from '../api/tipos';
import {
  marcarErroEvidencia,
  salvarEvidenciaLocal,
  salvarEvidenciaServidor,
} from '../db/entregas';
import { MAX_TENTATIVAS } from '../utils/backoff';
import { estaConectado } from './conexao';
import {
  enfileirarOperacao,
  listarItensFila,
  registrarFalhaItem,
  removerItemFila,
} from './fila';
import type { ExecutarAutenticado, ResultadoSincronizacao } from './lotes';

export type EvidenciaPendentePayload = {
  entregaId: number;
  latitude: number;
  longitude: number;
  observacao?: string;
  fotoUri: string;
  fotoNome: string;
  fotoTipo: string;
};

/** Salva a evidência localmente e agenda o PUT multipart idempotente. */
export async function agendarEnvioEvidencia(
  dados: EvidenciaPendentePayload,
): Promise<void> {
  await salvarEvidenciaLocal(
    dados.entregaId,
    dados.latitude,
    dados.longitude,
    dados.fotoUri,
    dados.observacao,
  );
  await enfileirarOperacao('enviar_evidencia', `entrega-${dados.entregaId}`, dados);
}

function mensagemDoErro(erro: unknown): string {
  return erro instanceof ApiError ? erro.message : 'Falha ao enviar a evidência.';
}

async function apagarArquivoLocal(uri: string): Promise<void> {
  if (!uri.startsWith('file:')) {
    return;
  }
  try {
    await FileSystem.deleteAsync(uri, { idempotent: true });
  } catch {
    // A limpeza do cache local é best-effort.
  }
}

/** Envia as evidências pendentes na ordem da fila. */
export async function sincronizarEvidencias(
  executarAutenticado: ExecutarAutenticado,
): Promise<ResultadoSincronizacao> {
  const resultado: ResultadoSincronizacao = {
    offline: false,
    enviados: 0,
    conflitos: 0,
    falhas: 0,
  };

  if (!(await estaConectado())) {
    resultado.offline = true;
    return resultado;
  }

  const pendentes = await listarItensFila('enviar_evidencia', true);

  for (const item of pendentes) {
    try {
      const payload = JSON.parse(item.payload) as EvidenciaPendentePayload;
      const entrega: Entrega = await executarAutenticado((token) =>
        enviarEvidencia(token, payload.entregaId, {
          latitude: payload.latitude,
          longitude: payload.longitude,
          ...(payload.observacao ? { observacao: payload.observacao } : {}),
          foto: {
            uri: payload.fotoUri,
            nome: payload.fotoNome,
            tipo: payload.fotoTipo,
          },
        }),
      );

      await salvarEvidenciaServidor(entrega);
      await removerItemFila(item.id);
      await apagarArquivoLocal(payload.fotoUri);
      resultado.enviados += 1;
    } catch (erro) {
      const mensagem = mensagemDoErro(erro);
      let payload: EvidenciaPendentePayload | null = null;

      try {
        payload = JSON.parse(item.payload) as EvidenciaPendentePayload;
      } catch {
        // Payload corrompido: a operação inválida é descartada abaixo.
      }

      if (erroDefinitivo(erro)) {
        if (payload) {
          await marcarErroEvidencia(payload.entregaId, mensagem);
        }
        await removerItemFila(item.id);
      } else if (item.tentativas + 1 >= MAX_TENTATIVAS) {
        if (payload) {
          await marcarErroEvidencia(
            payload.entregaId,
            `Falha após ${MAX_TENTATIVAS} tentativas: ${mensagem}`,
          );
        }
        await removerItemFila(item.id);
      } else {
        await registrarFalhaItem(item.id, mensagem);
      }

      resultado.falhas += 1;
    }
  }

  return resultado;
}
