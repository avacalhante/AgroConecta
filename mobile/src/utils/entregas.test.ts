import { proximosStatusEntrega, transicaoPermitida } from './entregas';

describe('transições de entrega', () => {
  it('permite Pendente para EmRota ou Cancelada', () => {
    expect(proximosStatusEntrega('Pendente')).toEqual(['EmRota', 'Cancelada']);
    expect(transicaoPermitida('Pendente', 'EmRota')).toBe(true);
    expect(transicaoPermitida('Pendente', 'Cancelada')).toBe(true);
  });

  it('permite EmRota para Entregue ou Falha', () => {
    expect(transicaoPermitida('EmRota', 'Entregue')).toBe(true);
    expect(transicaoPermitida('EmRota', 'Falha')).toBe(true);
  });

  it('bloqueia saltos e estados finais', () => {
    expect(transicaoPermitida('Pendente', 'Entregue')).toBe(false);
    expect(proximosStatusEntrega('Entregue')).toEqual([]);
    expect(proximosStatusEntrega('Falha')).toEqual([]);
    expect(proximosStatusEntrega('Cancelada')).toEqual([]);
  });
});
