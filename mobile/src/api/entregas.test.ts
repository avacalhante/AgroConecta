import * as FileSystem from 'expo-file-system/legacy';

import { enviarEvidencia } from './entregas';
import { ApiError } from './http';

jest.mock('expo-file-system/legacy', () => ({
  uploadAsync: jest.fn(),
  FileSystemUploadType: { MULTIPART: 1 },
}));

jest.mock('./fonte', () => ({ usarApiFake: false }));

const mockUpload = FileSystem.uploadAsync as jest.Mock;

const ENTRADA = {
  latitude: -23.651782,
  longitude: -46.611898,
  foto: {
    uri: 'file:///documentos/evidencias/entrega-1.jpg',
    nome: 'evidencia-1.jpg',
    tipo: 'image/jpeg',
  },
};

const ENTREGA = {
  id: 1,
  pedidoId: 1,
  status: 'Entregue',
  dataPrevista: '2026-11-05',
  dataRealizada: '2026-11-10T00:32:20Z',
  latitude: ENTRADA.latitude,
  longitude: ENTRADA.longitude,
  fotoUrl: '/uploads/entregas/foto.jpg',
  observacao: null,
};

describe('enviarEvidencia', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('usa o upload multipart nativo com o arquivo e as coordenadas', async () => {
    mockUpload.mockResolvedValue({ status: 200, body: JSON.stringify(ENTREGA) });

    const resultado = await enviarEvidencia('token-teste', 1, ENTRADA);

    expect(mockUpload).toHaveBeenCalledWith(
      expect.stringContaining('/entregas/1/evidencia'),
      ENTRADA.foto.uri,
      expect.objectContaining({
        httpMethod: 'PUT',
        fieldName: 'file',
        mimeType: 'image/jpeg',
        parameters: {
          latitude: String(ENTRADA.latitude),
          longitude: String(ENTRADA.longitude),
        },
        headers: expect.objectContaining({
          Authorization: 'Bearer token-teste',
        }),
      }),
    );
    expect(resultado).toEqual(ENTREGA);
  });

  it('converte a resposta RFC 7807 em ApiError', async () => {
    mockUpload.mockResolvedValue({
      status: 400,
      body: JSON.stringify({ detail: 'O arquivo da evidência é obrigatório.' }),
    });

    await expect(enviarEvidencia('token-teste', 1, ENTRADA)).rejects.toMatchObject({
      status: 400,
      message: 'O arquivo da evidência é obrigatório.',
    });
  });

  it('exige a foto antes de abrir o upload', async () => {
    await expect(
      enviarEvidencia('token-teste', 1, {
        latitude: ENTRADA.latitude,
        longitude: ENTRADA.longitude,
      }),
    ).rejects.toBeInstanceOf(ApiError);

    expect(mockUpload).not.toHaveBeenCalled();
  });
});
