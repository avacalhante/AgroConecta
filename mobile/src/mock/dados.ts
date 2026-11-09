import type { EntregaStatus, LoteStatus } from '../types/dominio';

/**
 * Dados de exemplo alinhados ao seed (`contracts/seed.md`).
 * A tarefa B3 adiciona a camada de mock do cliente e a B4/B5 substitui as telas.
 */

export type LoteExemplo = {
  id: string;
  produto: string;
  codigoRastreio: string;
  quantidade: number;
  unidade: string;
  dataColheita: string;
  status: LoteStatus;
};

export const LOTES_EXEMPLO: LoteExemplo[] = [
  {
    id: '1',
    produto: 'Tomate italiano',
    codigoRastreio: '550e8400-e29b-41d4-a716-446655440001',
    quantidade: 120,
    unidade: 'kg',
    dataColheita: '2026-11-01',
    status: 'Disponivel',
  },
  {
    id: '2',
    produto: 'Alface crespa',
    codigoRastreio: '550e8400-e29b-41d4-a716-446655440002',
    quantidade: 80,
    unidade: 'un',
    dataColheita: '2026-11-02',
    status: 'Reservado',
  },
];

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

export function buscarLote(id: string): LoteExemplo | undefined {
  return LOTES_EXEMPLO.find((lote) => lote.id === id);
}

export function buscarEntrega(id: number): EntregaExemplo | undefined {
  return ENTREGAS_EXEMPLO.find((entrega) => entrega.id === id);
}
