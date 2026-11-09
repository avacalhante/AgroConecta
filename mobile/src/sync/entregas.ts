import { alterarStatusEntrega, listarMinhasEntregas } from '../api/entregas';
import { ApiError, erroDefinitivo } from '../api/http';
import type { Entrega, EntregaStatusRequest } from '../api/tipos';
import {
  aplicarStatusLocal,
  buscarEntregaLocal,
  reverterPendenciaComErro,
  salvarEntregaServidor,
  upsertEntregasRemotas,
} from '../db/entregas';
import type { EntregaStatus } from '../types/dominio';
import { MAX_TENTATIVAS } from '../utils/backoff';
import { transicaoPermitida } from '../utils/entregas';
import { estaConectado } from './conexao';
import {
  enfileirarOperacao,
  listarItensFila,
  registrarFalhaItem,
  removerItemFila,
} from './fila';
import type { ExecutarAutenticado, ResultadoSincronizacao } from './lotes';

type StatusPendentePayload = EntregaStatusRequest & {
  id: number;
};

/**
 * Atualiza o status no SQLite e enfileira o PATCH idempotente.
 * Repetir o status atual não gera nova operação.
 */
export async function agendarStatusEntrega(
  id: number,
  status: EntregaStatus,
  ocorridoEm = new Date().toISOString(),
  observacao?: string,
): Promise<void> {
  const entrega = await buscarEntregaLocal(id);
  if (!entrega) {
    throw new Error('Entrega não encontrada.');
  }
  if (entrega.status === status) {
    return;
  }
  if (!transicaoPermitida(entrega.status, status)) {
    throw new Error('Transição de status inválida.');
  }

  await aplicarStatusLocal(id, status, ocorridoEm, observacao);
  await enfileirarOperacao('atualizar_status_entrega', `entrega-${id}-${ocorridoEm}`, {
    id,
    status,
    ocorridoEm,
    observacao,
  });
}

function mensagemDoErro(erro: unknown): string {
  return erro instanceof ApiError ? erro.message : 'Falha ao sincronizar a entrega.';
}

/** Envia os PATCHs pendentes e depois puxa a lista atualizada do motorista. */
export async function sincronizarEntregas(
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

  const pendentes = await listarItensFila('atualizar_status_entrega', true);

  for (const item of pendentes) {
    try {
      const payload = JSON.parse(item.payload) as StatusPendentePayload;
      const corpo: EntregaStatusRequest = {
        status: payload.status,
        ocorridoEm: payload.ocorridoEm,
        ...(payload.observacao !== undefined ? { observacao: payload.observacao } : {}),
      };

      const entrega: Entrega = await executarAutenticado((token) =>
        alterarStatusEntrega(token, payload.id, corpo),
      );
      await salvarEntregaServidor(entrega);
      await removerItemFila(item.id);
      resultado.enviados += 1;
    } catch (erro) {
      const mensagem = mensagemDoErro(erro);

      if (erroDefinitivo(erro)) {
        // 400/403/404/422 não devem ser repetidos; volta ao estado do servidor
        // na próxima leitura e mantém o conflito visível.
        try {
          const payload = JSON.parse(item.payload) as StatusPendentePayload;
          await reverterPendenciaComErro(payload.id, mensagem);
        } catch {
          // Payload corrompido: remove a operação inválida abaixo.
        }
        await removerItemFila(item.id);
      } else if (item.tentativas + 1 >= MAX_TENTATIVAS) {
        try {
          const payload = JSON.parse(item.payload) as StatusPendentePayload;
          await reverterPendenciaComErro(
            payload.id,
            `Falha após ${MAX_TENTATIVAS} tentativas: ${mensagem}`,
          );
        } catch {
          // Payload corrompido: apenas descarta a operação.
        }
        await removerItemFila(item.id);
      } else {
        await registrarFalhaItem(item.id, mensagem);
      }

      resultado.falhas += 1;
    }
  }

  try {
    const entregas = await executarAutenticado((token) => listarMinhasEntregas(token));
    await upsertEntregasRemotas(entregas);
  } catch {
    resultado.falhas += 1;
  }

  return resultado;
}
