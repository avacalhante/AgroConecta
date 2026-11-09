import type { EntregaStatus } from '../types/dominio';
import { usarApiFake } from './fonte';
import { requisitar } from './http';
import {
  alterarStatusEntregaFake,
  enviarEvidenciaFake,
  listarMinhasEntregasFake,
} from './mock/servidorFake';
import type { Entrega, EntregaStatusRequest, EvidenciaRequest } from './tipos';

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
export function enviarEvidencia(
  token: string,
  id: number,
  dados: EvidenciaRequest,
): Promise<Entrega> {
  if (usarApiFake) {
    return enviarEvidenciaFake(token, id, dados);
  }

  const formulario = new FormData();
  formulario.append('latitude', String(dados.latitude));
  formulario.append('longitude', String(dados.longitude));
  if (dados.observacao) {
    formulario.append('observacao', dados.observacao);
  }
  if (dados.foto) {
    formulario.append(
      'file',
      {
        uri: dados.foto.uri,
        name: dados.foto.nome,
        type: dados.foto.tipo,
      } as unknown as Blob,
    );
  }

  return requisitar<Entrega>(`/entregas/${id}/evidencia`, {
    metodo: 'PUT',
    formData: formulario,
    token,
  });
}
