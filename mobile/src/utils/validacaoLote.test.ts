import {
  converterQuantidade,
  dataIsoValida,
  hojeIsoLocal,
  validarNovoLote,
} from './validacaoLote';

describe('validação de novo lote', () => {
  it('aceita produto, data passada e quantidade positiva', () => {
    const erros = validarNovoLote({
      produtoId: 1,
      dataColheita: '2026-11-01',
      quantidade: '120',
    });

    expect(erros).toEqual({});
  });

  it('exige a seleção de produto', () => {
    const erros = validarNovoLote({
      produtoId: null,
      dataColheita: '2026-11-01',
      quantidade: '10',
    });

    expect(erros.produtoId).toBeDefined();
  });

  it('rejeita data inválida e data inexistente no calendário', () => {
    expect(dataIsoValida('01/11/2026')).toBe(false);
    expect(dataIsoValida('2026-02-30')).toBe(false);

    const erros = validarNovoLote({
      produtoId: 1,
      dataColheita: '2026-13-01',
      quantidade: '10',
    });
    expect(erros.dataColheita).toBeDefined();
  });

  it('rejeita data futura', () => {
    const erros = validarNovoLote({
      produtoId: 1,
      dataColheita: '2999-01-01',
      quantidade: '10',
    });

    expect(erros.dataColheita).toContain('futura');
  });

  it('aceita a data de hoje', () => {
    const erros = validarNovoLote({
      produtoId: 1,
      dataColheita: hojeIsoLocal(),
      quantidade: '1',
    });

    expect(erros).toEqual({});
  });

  it('rejeita quantidade zero, negativa ou não numérica', () => {
    for (const quantidade of ['0', '-5', 'abc', '']) {
      const erros = validarNovoLote({
        produtoId: 1,
        dataColheita: '2026-11-01',
        quantidade,
      });
      expect(erros.quantidade).toBeDefined();
    }
  });

  it('converte vírgula decimal', () => {
    expect(converterQuantidade('3,5')).toBe(3.5);
    expect(converterQuantidade(' 12 ')).toBe(12);
  });
});
