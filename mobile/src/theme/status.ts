import type { StatusValor } from '../types/dominio';
import { colors } from './tokens';

export type StatusTone = 'success' | 'warning' | 'danger' | 'info' | 'neutral';

type ToneColors = {
  background: string;
  text: string;
  border: string;
};

/** Cores de cada tom de status, com contraste AA sobre o fundo claro. */
export const statusToneColors: Record<StatusTone, ToneColors> = {
  success: {
    background: colors.successLight,
    text: colors.success,
    border: colors.success,
  },
  warning: {
    background: colors.warningLight,
    text: colors.warning,
    border: colors.warning,
  },
  danger: {
    background: colors.dangerLight,
    text: colors.danger,
    border: colors.danger,
  },
  info: {
    background: colors.infoLight,
    text: colors.info,
    border: colors.info,
  },
  neutral: {
    background: colors.surfaceMuted,
    text: colors.textMuted,
    border: colors.borderStrong,
  },
};

export type StatusMeta = {
  label: string;
  tone: StatusTone;
};

/** Rótulo em pt-BR e tom visual de cada status do contrato. */
export const statusMeta: Record<StatusValor, StatusMeta> = {
  Disponivel: { label: 'Disponível', tone: 'success' },
  Reservado: { label: 'Reservado', tone: 'warning' },
  Esgotado: { label: 'Esgotado', tone: 'neutral' },
  Pendente: { label: 'Pendente', tone: 'warning' },
  EmRota: { label: 'Em rota', tone: 'info' },
  Entregue: { label: 'Entregue', tone: 'success' },
  Falha: { label: 'Falha', tone: 'danger' },
  Cancelada: { label: 'Cancelada', tone: 'neutral' },
  Rascunho: { label: 'Rascunho', tone: 'neutral' },
  AguardandoAprovacao: { label: 'Aguardando aprovação', tone: 'warning' },
  Aprovado: { label: 'Aprovado', tone: 'success' },
  EmSeparacao: { label: 'Em separação', tone: 'info' },
  Cancelado: { label: 'Cancelado', tone: 'danger' },
};
