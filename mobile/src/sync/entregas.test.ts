import { ApiError } from '../api/http';
import type { Entrega } from '../api/tipos';
import type { EntregaLocal } from '../db/entregas';
import type { ExecutarAutenticado } from './lotes';
import { agendarStatusEntrega, sincronizarEntregas } from './entregas';

const mockAlterarStatusEntrega = jest.fn();
const mockListarMinhasEntregas = jest.fn();
const mockAplicarStatusLocal = jest.fn();
const mockBuscarEntregaLocal = jest.fn();
const mockMarcarErro = jest.fn();
const mockReverterPendenciaComErro = jest.fn();
const mockSalvarEntregaServidor = jest.fn();
const mockUpsertEntregasRemotas = jest.fn();
const mockEstaConectado = jest.fn();
const mockListarItensFila = jest.fn();
const mockEnfileirarOperacao = jest.fn();
const mockRegistrarFalhaItem = jest.fn();
const mockRemoverItemFila = jest.fn();

jest.mock('../api/entregas', () => ({
  alterarStatusEntrega: (...args: unknown[]) => mockAlterarStatusEntrega(...args),
  listarMinhasEntregas: (...args: unknown[]) => mockListarMinhasEntregas(...args),
}));

jest.mock('../db/entregas', () => ({
  aplicarStatusLocal: (...args: unknown[]) => mockAplicarStatusLocal(...args),
  buscarEntregaLocal: (...args: unknown[]) => mockBuscarEntregaLocal(...args),
  marcarErroSincronizacaoEntrega: (...args: unknown[]) => mockMarcarErro(...args),
  reverterPendenciaComErro: (...args: unknown[]) => mockReverterPendenciaComErro(...args),
  salvarEntregaServidor: (...args: unknown[]) => mockSalvarEntregaServidor(...args),
  upsertEntregasRemotas: (...args: unknown[]) => mockUpsertEntregasRemotas(...args),
}));

jest.mock('./conexao', () => ({
  estaConectado: (...args: unknown[]) => mockEstaConectado(...args),
}));

jest.mock('./fila', () => ({
  enfileirarOperacao: (...args: unknown[]) => mockEnfileirarOperacao(...args),
  listarItensFila: (...args: unknown[]) => mockListarItensFila(...args),
  registrarFalhaItem: (...args: unknown[]) => mockRegistrarFalhaItem(...args),
  removerItemFila: (...args: unknown[]) => mockRemoverItemFila(...args),
}));

const OCOBRIDO = '2026-11-09T12:00:00.000Z';

const ENTREGA_LOCAL: EntregaLocal = {
  id: 1,
  pedidoId: 1,
  status: 'Pendente',
  dataPrevista: '2026-11-05',
  dataRealizada: null,
  latitude: null,
  longitude: null,
  fotoUrl: null,
  observacao: null,
  pendente: false,
  erroSincronizacao: null,
};

const ENTREGA_SERVIDOR: Entrega = {
  id: 1,
  pedidoId: 1,
  status: 'EmRota',
  dataPrevista: '2026-11-05',
  dataRealizada: null,
  latitude: null,
  longitude: null,
  fotoUrl: null,
  observacao: null,
};

const ITEM_FILA = {
  id: 7,
  tipo: 'atualizar_status_entrega' as const,
  referencia: `entrega-1-${OCOBRIDO}`,
  payload: JSON.stringify({
    id: 1,
    status: 'EmRota',
    ocorridoEm: OCOBRIDO,
  }),
  criadoEm: OCOBRIDO,
  tentativas: 0,
  ultimoErro: null,
};

const executar: ExecutarAutenticado = (acao) => acao('token-teste');

describe('status de entrega local-first', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockBuscarEntregaLocal.mockResolvedValue(ENTREGA_LOCAL);
    mockAplicarStatusLocal.mockResolvedValue(undefined);
    mockEstaConectado.mockResolvedValue(true);
    mockListarItensFila.mockResolvedValue([]);
    mockListarMinhasEntregas.mockResolvedValue([]);
    mockSalvarEntregaServidor.mockResolvedValue(undefined);
    mockReverterPendenciaComErro.mockResolvedValue(undefined);
    mockUpsertEntregasRemotas.mockResolvedValue(undefined);
    mockEnfileirarOperacao.mockResolvedValue(undefined);
    mockRegistrarFalhaItem.mockResolvedValue(undefined);
    mockRemoverItemFila.mockResolvedValue(undefined);
  });

  it('salva o status local e enfileira o PATCH', async () => {
    await agendarStatusEntrega(1, 'EmRota', OCOBRIDO);

    expect(mockAplicarStatusLocal).toHaveBeenCalledWith(1, 'EmRota', OCOBRIDO, undefined);
    expect(mockEnfileirarOperacao).toHaveBeenCalledWith(
      'atualizar_status_entrega',
      `entrega-1-${OCOBRIDO}`,
      expect.objectContaining({ id: 1, status: 'EmRota' }),
    );
  });

  it('não enfileira quando o status é o mesmo', async () => {
    await agendarStatusEntrega(1, 'Pendente', OCOBRIDO);

    expect(mockAplicarStatusLocal).not.toHaveBeenCalled();
    expect(mockEnfileirarOperacao).not.toHaveBeenCalled();
  });

  it('bloqueia transição inválida', async () => {
    await expect(agendarStatusEntrega(1, 'Entregue', OCOBRIDO)).rejects.toThrow(
      'Transição de status inválida.',
    );
  });

  it('envia o PATCH e atualiza a entrega local', async () => {
    mockListarItensFila.mockResolvedValue([ITEM_FILA]);
    mockAlterarStatusEntrega.mockResolvedValue(ENTREGA_SERVIDOR);

    const resultado = await sincronizarEntregas(executar);

    expect(mockAlterarStatusEntrega).toHaveBeenCalledWith('token-teste', 1, {
      status: 'EmRota',
      ocorridoEm: OCOBRIDO,
    });
    expect(mockSalvarEntregaServidor).toHaveBeenCalledWith(ENTREGA_SERVIDOR);
    expect(mockRemoverItemFila).toHaveBeenCalledWith(7);
    expect(resultado.enviados).toBe(1);
  });

  it('trata 422 como conflito definitivo e mantém o erro visível', async () => {
    mockListarItensFila.mockResolvedValue([ITEM_FILA]);
    mockAlterarStatusEntrega.mockRejectedValue(
      new ApiError(422, 'Transição não permitida.'),
    );

    const resultado = await sincronizarEntregas(executar);

    expect(mockRegistrarFalhaItem).not.toHaveBeenCalled();
    expect(mockReverterPendenciaComErro).toHaveBeenCalledWith(
      1,
      'Transição não permitida.',
    );
    expect(mockRemoverItemFila).toHaveBeenCalledWith(7);
    expect(resultado.falhas).toBe(1);
  });

  it('mantém 5xx na fila com backoff', async () => {
    mockListarItensFila.mockResolvedValue([ITEM_FILA]);
    mockAlterarStatusEntrega.mockRejectedValue(new ApiError(500, 'Erro interno.'));

    const resultado = await sincronizarEntregas(executar);

    expect(mockRegistrarFalhaItem).toHaveBeenCalledWith(7, 'Erro interno.');
    expect(mockRemoverItemFila).not.toHaveBeenCalled();
    expect(mockReverterPendenciaComErro).not.toHaveBeenCalled();
    expect(resultado.falhas).toBe(1);
  });

  it('desiste após o limite de tentativas', async () => {
    mockListarItensFila.mockResolvedValue([{ ...ITEM_FILA, tentativas: 4 }]);
    mockAlterarStatusEntrega.mockRejectedValue(new ApiError(503, 'Indisponível.'));

    await sincronizarEntregas(executar);

    expect(mockRegistrarFalhaItem).not.toHaveBeenCalled();
    expect(mockReverterPendenciaComErro).toHaveBeenCalledWith(
      1,
      'Falha após 5 tentativas: Indisponível.',
    );
    expect(mockRemoverItemFila).toHaveBeenCalledWith(7);
  });

  it('não sincroniza offline', async () => {
    mockEstaConectado.mockResolvedValue(false);

    const resultado = await sincronizarEntregas(executar);

    expect(resultado.offline).toBe(true);
    expect(mockListarItensFila).not.toHaveBeenCalled();
  });

  it('puxa as entregas remotas após a fila', async () => {
    mockListarMinhasEntregas.mockResolvedValue([ENTREGA_SERVIDOR]);

    await sincronizarEntregas(executar);

    expect(mockUpsertEntregasRemotas).toHaveBeenCalledWith([ENTREGA_SERVIDOR]);
  });
});
