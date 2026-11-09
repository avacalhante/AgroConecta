import { calcularBackoffMs, MAX_TENTATIVAS } from './backoff';

describe('backoff de sincronização', () => {
  it('cresce exponencialmente a partir de 1 segundo', () => {
    expect(calcularBackoffMs(0)).toBe(1_000);
    expect(calcularBackoffMs(1)).toBe(2_000);
    expect(calcularBackoffMs(2)).toBe(4_000);
    expect(calcularBackoffMs(4)).toBe(16_000);
  });

  it('limita o atraso a 5 minutos', () => {
    expect(calcularBackoffMs(10)).toBe(5 * 60 * 1000);
    expect(calcularBackoffMs(50)).toBe(5 * 60 * 1000);
  });

  it('define o máximo de cinco tentativas', () => {
    expect(MAX_TENTATIVAS).toBe(5);
  });
});
