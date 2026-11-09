export const MAX_TENTATIVAS = 5;

/** Backoff exponencial: 1 s, 2 s, 4 s… limitado a 5 minutos. */
export function calcularBackoffMs(tentativas: number): number {
  return Math.min(2 ** tentativas * 1000, 5 * 60 * 1000);
}
