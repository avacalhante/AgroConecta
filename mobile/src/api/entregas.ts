import * as FileSystem from 'expo-file-system/legacy';

import type { EntregaStatus } from '../types/dominio';
import { apiConfig } from './config';
import { usarApiFake } from './fonte';
import { ApiError, requisitar } from './http';
import {
  alterarStatusEntregaFake,
  enviarEvidenciaFake,
  listarMinhasEntregasFake,
} from './mock/servidorFake';
import type {
  Entrega,
  EntregaStatusRequest,
  EvidenciaRequest,
  ProblemDetails,
} from './tipos';

export function listarMinhasEntregas(
  token: string,
  status?: EntregaStatus,
): Promise<Entrega[]> {
  if (usarApiFake) {
    return listarMinhasEntregasFake(token, status);
  }

  const consulta = status ? `?status=${encodeURIComponent(status)}` : '';
  return requisitar<Entrega[]>(`/entregas/minhas${consulta}`, { token });
}

/** Operação idempotente: repetir o mesmo status não gera efeito colateral. */
export function alterarStatusEntrega(
  token: string,
  id: number,
  corpo: EntregaStatusRequest,
): Promise<Entrega> {
  if (usarApiFake) {
    return alterarStatusEntregaFake(token, id, corpo);
  }

  return requisitar<Entrega>(`/entregas/${id}/status`, {
    metodo: 'PATCH',
    corpo,
    token,
  });
}

/** Operação idempotente: a última evidência enviada prevalece. */
export async function enviarEvidencia(
  token: string,
  id: number,
  dados: EvidenciaRequest,
): Promise<Entrega> {
  if (usarApiFake) {
    return enviarEvidenciaFake(token, id, dados);
  }

  if (!dados.foto) {
    throw new ApiError(422, 'A foto da evidência é obrigatória.');
  }

  const parametros: Record<string, string> = {
    latitude: String(dados.latitude),
    longitude: String(dados.longitude),
  };
  if (dados.observacao) {
    parametros.observacao = dados.observacao;
  }

  const url = `${apiConfig.baseUrl.replace(/\/+$/, '')}/entregas/${id}/evidencia`;

  try {
    // O upload nativo evita falhas do FormData com arquivos no Android.
    const resposta = await FileSystem.uploadAsync(url, dados.foto.uri, {
      httpMethod: 'PUT',
      uploadType: FileSystem.FileSystemUploadType.MULTIPART,
      fieldName: 'file',
      mimeType: dados.foto.tipo,
      parameters: parametros,
      headers: {
        Accept: 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });

    if (resposta.status < 200 || resposta.status >= 300) {
      let problema: ProblemDetails | undefined;
      try {
        problema = JSON.parse(resposta.body) as ProblemDetails;
      } catch {
        problema = undefined;
      }
      throw new ApiError(
        resposta.status,
        problema?.detail ??
          problema?.title ??
          'Não foi possível enviar a evidência.',
        problema,
      );
    }

    return JSON.parse(resposta.body) as Entrega;
  } catch (erro) {
    if (erro instanceof ApiError) {
      throw erro;
    }
    throw new ApiError(
      0,
      'Sem conexão com o servidor. Verifique a rede e tente novamente.',
    );
  }
}
