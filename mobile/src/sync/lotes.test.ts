import { ApiError } from '../api/http';
import type { Lote, LotePage, ProdutoPage } from '../api/tipos';
import { sincronizarLotes, type ExecutarAutenticado } from './lotes';

const mockCriarLote = jest.fn();
const mockListarMeusLotes = jest.fn();
const mockListarProdutos = jest.fn();
const mockMarcarLoteSincronizado = jest.fn();
const mockUpsertLotesRemotos = jest.fn();
const mockSalvarProdutosRemotos = jest.fn();
const mockEstaConectado = jest.fn();
const mockListarItensFila = jest.fn();
const mockRegistrarFalhaItem = jest.fn();
const mockRemoverItemFila = jest.fn();

jest.mock('../api/lotes', () => ({
  criarLote: (...args: unknown[]) => mockCriarLote(...args),
  listarMeusLotes: (...args: unknown[]) => mockListarMeusLotes(...args),
}));

jest.mock('../api/produtos', () => ({
  listarProdutos: (...args: unknown[]) => mockListarProdutos(...args),
}));

jest.mock('../db/lotes', () => ({
  salvarLoteLocal: jest.fn(),
  marcarLoteSincronizado: (...args: unknown[]) => mockMarcarLoteSincronizado(...args),
  upsertLotesRemotos: (...args: unknown[]) => mockUpsertLotesRemotos(...args),
}));

jest.mock('../db/produtos', () => ({
  salvarProdutosRemotos: (...args: unknown[]) => mockSalvarProdutosRemotos(...args),
}));

jest.mock('./conexao', () => ({
  estaConectado: (...args: unknown[]) => mockEstaConectado(...args),
}));

jest.mock('./fila', () => ({
  enfileirarOperacao: jest.fn(),
  listarItensFila: (...args: unknown[]) => mockListarItensFila(...args),
  registrarFalhaItem: (...args: unknown[]) => mockRegistrarFalhaItem(...args),
  removerItemFila: (...args: unknown[]) => mockRemoverItemFila(...args),
}));

const CODIGO = '550e8400-e29b-41d4-a716-446655440099';

const ITEM = {
  id: 10,
  tipo: 'criar_lote' as const,
  referencia: CODIGO,
  payload: JSON.stringify({
    produtoId: 1,
    codigoRastreio: CODIGO,
    dataColheita: '2026-11-01',
    quantidade: 15,
    produtoNome: 'Tomate italiano',
  }),
  criadoEm: '2026-11-01T10:00:00.000Z',
  tentativas: 0,
  ultimoErro: null,
};

const LOTE: Lote = {
  id: 42,
  produtorId: 2,
  produtoId: 1,
  produtoNome: 'Tomate italiano',
  codigoRastreio: CODIGO,
  dataColheita: '2026-11-01',
  quantidade: 15,
  status: 'Disponivel',
  criadoEm: '2026-11-01T10:00:00.000Z',
};

const PAGINA_VAZIA: LotePage = { items: [], page: 1, pageSize: 20, total: 0 };
const PRODUTOS_VAZIOS: ProdutoPage = { items: [], page: 1, pageSize: 20, total: 0 };

const executar: ExecutarAutenticado = (acao) => acao('token-teste');

describe('sincronizarLotes', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockEstaConectado.mockResolvedValue(true);
    mockListarItensFila.mockResolvedValue([]);
    mockListarMeusLotes.mockResolvedValue(PAGINA_VAZIA);
    mockListarProdutos.mockResolvedValue(PRODUTOS_VAZIOS);
    mockMarcarLoteSincronizado.mockResolvedValue(undefined);
    mockUpsertLotesRemotos.mockResolvedValue(undefined);
    mockSalvarProdutosRemotos.mockResolvedValue(undefined);
    mockRegistrarFalhaItem.mockResolvedValue(undefined);
    mockRemoverItemFila.mockResolvedValue(undefined);
  });

  it('não sincroniza quando está offline', async () => {
    mockEstaConectado.mockResolvedValue(false);

    const resultado = await sincronizarLotes(executar);

    expect(resultado.offline).toBe(true);
    expect(mockListarItensFila).not.toHaveBeenCalled();
    expect(mockListarMeusLotes).not.toHaveBeenCalled();
  });

  it('envia o lote pendente e o marca como sincronizado', async () => {
    mockListarItensFila.mockResolvedValue([ITEM]);
    mockCriarLote.mockResolvedValue(LOTE);

    const resultado = await sincronizarLotes(executar);

    expect(mockCriarLote).toHaveBeenCalledWith('token-teste', {
      produtoId: 1,
      codigoRastreio: CODIGO,
      dataColheita: '2026-11-01',
      quantidade: 15,
    });
    expect(mockMarcarLoteSincronizado).toHaveBeenCalledWith(CODIGO, 42);
    expect(mockRemoverItemFila).toHaveBeenCalledWith(10);
    expect(resultado.enviados).toBe(1);
    expect(resultado.falhas).toBe(0);
  });

  it('trata 409 como lote já sincronizado', async () => {
    mockListarItensFila.mockResolvedValue([ITEM]);
    mockCriarLote.mockRejectedValue(new ApiError(409, 'Código já existente.'));

    const resultado = await sincronizarLotes(executar);

    expect(mockMarcarLoteSincronizado).toHaveBeenCalledWith(CODIGO, null);
    expect(mockRemoverItemFila).toHaveBeenCalledWith(10);
    expect(resultado.conflitos).toBe(1);
    expect(resultado.falhas).toBe(0);
  });

  it('mantém a falha de rede na fila para nova tentativa', async () => {
    mockListarItensFila.mockResolvedValue([ITEM]);
    mockCriarLote.mockRejectedValue(new ApiError(0, 'Sem conexão.'));

    const resultado = await sincronizarLotes(executar);

    expect(mockRegistrarFalhaItem).toHaveBeenCalledWith(10, 'Sem conexão.');
    expect(mockRemoverItemFila).not.toHaveBeenCalled();
    expect(resultado.falhas).toBe(1);
  });

  it('puxa a lista de lotes e produtos após enviar', async () => {
    mockListarMeusLotes.mockResolvedValue({ ...PAGINA_VAZIA, items: [LOTE] });

    await sincronizarLotes(executar);

    expect(mockUpsertLotesRemotos).toHaveBeenCalledWith([LOTE]);
    expect(mockSalvarProdutosRemotos).toHaveBeenCalled();
  });
});
