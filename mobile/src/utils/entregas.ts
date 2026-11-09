import type { EntregaStatus } from '../types/dominio';

/** Transições permitidas em `contracts/enums.md`. */
export const TRANSICOES_ENTREGA: Record<EntregaStatus, EntregaStatus[]> = {
  Pendente: ['EmRota', 'Cancelada'],
  EmRota: ['Entregue', 'Falha'],
  Entregue: [],
  Falha: [],
  Cancelada: [],
};

export function proximosStatusEntrega(status: EntregaStatus): EntregaStatus[] {
  return TRANSICOES_ENTREGA[status];
}

export function transicaoPermitida(de: EntregaStatus, para: EntregaStatus): boolean {
  return TRANSICOES_ENTREGA[de].includes(para);
}
