import type { EntregaStatus } from '../types/dominio';

/**
 * Dados de exemplo de entregas, alinhados ao seed (`contracts/seed.md`).
 * A tarefa B5 substitui as telas pelos dados locais e da API.
 */

export type EntregaExemplo = {
  id: number;
  pedidoId: number;
  destino: string;
  dataPrevista: string;
  status: EntregaStatus;
  observacao?: string;
};

export const ENTREGAS_EXEMPLO: EntregaExemplo[] = [
  {
    id: 1,
    pedidoId: 1,
    destino: 'Campinas/SP',
    dataPrevista: '2026-11-05',
    status: 'EmRota',
    observacao: 'Entregar na doca 2 até as 14h.',
  },
  {
    id: 2,
    pedidoId: 2,
    destino: 'Valinhos/SP',
    dataPrevista: '2026-11-06',
    status: 'Pendente',
  },
];

export function buscarEntrega(id: number): EntregaExemplo | undefined {
  return ENTREGAS_EXEMPLO.find((entrega) => entrega.id === id);
}
