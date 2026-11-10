import { ApiError } from '../api/http';
import type { Entrega } from '../api/tipos';
import type { ExecutarAutenticado } from './lotes';
import { agendarEnvioEvidencia, sincronizarEvidencias } from './evidencias';

const mockEnviarEvidencia = jest.fn();
const mockSalvarEvidenciaLocal = jest.fn();
const mockSalvarEvidenciaServidor = jest.fn();
const mockMarcarErroEvidencia = jest.fn();
const mockEstaConectado = jest.fn();
const mockListarItensFila = jest.fn();
const mockEnfileirarOperacao = jest.fn();
const mockRegistrarFalhaItem = jest.fn();
const mockRemoverItemFila = jest.fn();
const mockDeleteAsync = jest.fn();

jest.mock('../api/entregas', () => ({
  enviarEvidencia: (...args: unknown[]) => mockEnviarEvidencia(...args),
}));

jest.mock('../db/entregas', () => ({
  salvarEvidenciaLocal: (...args: unknown[]) => mockSalvarEvidenciaLocal(...args),
  salvarEvidenciaServidor: (...args: unknown[]) => mockSalvarEvidenciaServidor(...args),
  marcarErroEvidencia: (...args: unknown[]) => mockMarcarErroEvidencia(...args),
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

jest.mock('expo-file-system/legacy', () => ({
  deleteAsync: (...args: unknown[]) => mockDeleteAsync(...args),
}));

const CRIADO_EM = '2026-11-09T12:00:00.000Z';

const PAYLOAD = {
  entregaId: 1,
  latitude: -22.9068,
  longitude: -43.1729,
  observacao: 'Entrega conferida.',
  fotoUri: 'file:///documentos/evidencias/entrega-1.jpg',
  fotoNome: 'evidencia-1.jpg',
  fotoTipo: 'image/jpeg',
};

const ITEM_FILA = {
  id: 9,
  tipo: 'enviar_evidencia' as const,
  referencia: 'entrega-1',
  payload: JSON.stringify(PAYLOAD),
  criadoEm: CRIADO_EM,
  tentativas: 0,
  ultimoErro: null,
  proximaTentativaEm: null,
};

const ENTREGA_SERVIDOR: Entrega = {
  id: 1,
  pedidoId: 1,
  status: 'Entregue',
  dataPrevista: '2026-11-05',
  dataRealizada: CRIADO_EM,
  latitude: -22.9068,
  longitude: -43.1729,
  fotoUrl: 'https://api.local/evidencias/1.jpg',
  observacao: 'Entrega conferida.',
};

const executar: ExecutarAutenticado = (acao) => acao('token-teste');

describe('evidência local-first', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockSalvarEvidenciaLocal.mockResolvedValue(undefined);
    mockSalvarEvidenciaServidor.mockResolvedValue(undefined);
    mockMarcarErroEvidencia.mockResolvedValue(undefined);
    mockEstaConectado.mockResolvedValue(true);
    mockListarItensFila.mockResolvedValue([]);
    mockEnfileirarOperacao.mockResolvedValue(undefined);
    mockRegistrarFalhaItem.mockResolvedValue(undefined);
    mockRemoverItemFila.mockResolvedValue(undefined);
    mockDeleteAsync.mockResolvedValue(undefined);
  });

  it('salva a evidência local e enfileira o PUT', async () => {
    await agendarEnvioEvidencia(PAYLOAD);

    expect(mockSalvarEvidenciaLocal).toHaveBeenCalledWith(
      1,
      PAYLOAD.latitude,
      PAYLOAD.longitude,
      PAYLOAD.fotoUri,
      PAYLOAD.observacao,
    );
    expect(mockEnfileirarOperacao).toHaveBeenCalledWith(
      'enviar_evidencia',
      'entrega-1',
      PAYLOAD,
    );
  });

  it('envia multipart, atualiza a entrega e apaga o arquivo local', async () => {
    mockListarItensFila.mockResolvedValue([ITEM_FILA]);
    mockEnviarEvidencia.mockResolvedValue(ENTREGA_SERVIDOR);

    const resultado = await sincronizarEvidencias(executar);

    expect(mockEnviarEvidencia).toHaveBeenCalledWith('token-teste', 1, {
      latitude: PAYLOAD.latitude,
      longitude: PAYLOAD.longitude,
      observacao: PAYLOAD.observacao,
      foto: {
        uri: PAYLOAD.fotoUri,
        nome: PAYLOAD.fotoNome,
        tipo: PAYLOAD.fotoTipo,
      },
    });
    expect(mockSalvarEvidenciaServidor).toHaveBeenCalledWith(ENTREGA_SERVIDOR);
    expect(mockRemoverItemFila).toHaveBeenCalledWith(9);
    expect(mockDeleteAsync).toHaveBeenCalledWith(PAYLOAD.fotoUri, {
      idempotent: true,
    });
    expect(resultado.enviados).toBe(1);
  });

  it('não envia nada quando está offline', async () => {
    mockEstaConectado.mockResolvedValue(false);

    const resultado = await sincronizarEvidencias(executar);

    expect(mockEnviarEvidencia).not.toHaveBeenCalled();
    expect(resultado.offline).toBe(true);
  });

  it('trata 422 como definitivo e mantém a foto local', async () => {
    mockListarItensFila.mockResolvedValue([ITEM_FILA]);
    mockEnviarEvidencia.mockRejectedValue(
      new ApiError(400, 'Arquivo de imagem inválido.'),
    );

    const resultado = await sincronizarEvidencias(executar);

    expect(mockRegistrarFalhaItem).not.toHaveBeenCalled();
    expect(mockMarcarErroEvidencia).toHaveBeenCalledWith(
      1,
      'Arquivo de imagem inválido.',
    );
    expect(mockRemoverItemFila).toHaveBeenCalledWith(9);
    expect(mockDeleteAsync).not.toHaveBeenCalled();
    expect(resultado.falhas).toBe(1);
  });

  it('mantém 5xx na fila com backoff', async () => {
    mockListarItensFila.mockResolvedValue([ITEM_FILA]);
    mockEnviarEvidencia.mockRejectedValue(new ApiError(503, 'Indisponível.'));

    const resultado = await sincronizarEvidencias(executar);

    expect(mockRegistrarFalhaItem).toHaveBeenCalledWith(9, 'Indisponível.');
    expect(mockRemoverItemFila).not.toHaveBeenCalled();
    expect(mockMarcarErroEvidencia).not.toHaveBeenCalled();
    expect(resultado.falhas).toBe(1);
  });

  it('desiste após o limite de tentativas', async () => {
    mockListarItensFila.mockResolvedValue([{ ...ITEM_FILA, tentativas: 4 }]);
    mockEnviarEvidencia.mockRejectedValue(new ApiError(500, 'Erro interno.'));

    await sincronizarEvidencias(executar);

    expect(mockRegistrarFalhaItem).not.toHaveBeenCalled();
    expect(mockMarcarErroEvidencia).toHaveBeenCalledWith(
      1,
      'Falha após 5 tentativas: Erro interno.',
    );
    expect(mockRemoverItemFila).toHaveBeenCalledWith(9);
  });
});
