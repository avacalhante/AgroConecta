/**
 * Tipos de domínio espelhando os enums de `contracts/enums.md`.
 * A tarefa B3 passa a gerar os tipos do OpenAPI em `src/types/api.d.ts`;
 * estes aliases continuam válidos para o que o app consome direto.
 */

export type Papel = 'Admin' | 'Produtor' | 'Comprador' | 'Motorista';

export type LoteStatus = 'Disponivel' | 'Reservado' | 'Esgotado';

export type EntregaStatus = 'Pendente' | 'EmRota' | 'Entregue' | 'Falha' | 'Cancelada';

export type PedidoStatus =
  | 'Rascunho'
  | 'AguardandoAprovacao'
  | 'Aprovado'
  | 'EmSeparacao'
  | 'EmRota'
  | 'Entregue'
  | 'Cancelado';

/** Qualquer status que possa ser exibido em um badge. */
export type StatusValor = LoteStatus | EntregaStatus | PedidoStatus;
