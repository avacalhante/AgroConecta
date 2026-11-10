import {
  formatarData,
  formatarDataHora,
  formatarMoeda,
  formatarQuantidade,
} from './format';

describe('formatadores', () => {
  it('formata data ISO no padrão brasileiro', () => {
    expect(formatarData('2026-11-01')).toBe('01/11/2026');
  });

  it('retorna travessão para data inválida', () => {
    expect(formatarData('data-invalida')).toBe('—');
  });

  it('formata data e hora', () => {
    const texto = formatarDataHora('2026-11-05T14:30:00.000Z');
    expect(texto).toMatch(/^0?5\/11\/2026/);
  });

  it('formata quantidade com unidade', () => {
    expect(formatarQuantidade(120, 'kg')).toBe('120 kg');
    expect(formatarQuantidade(3.5, 'un')).toBe('3,5 un');
  });

  it('retorna travessão para quantidade inválida', () => {
    expect(formatarQuantidade(Number.NaN)).toBe('—');
  });

  it('formata valores em reais', () => {
    expect(formatarMoeda(425).replace(/\s/g, ' ')).toBe('R$ 425,00');
  });
});
