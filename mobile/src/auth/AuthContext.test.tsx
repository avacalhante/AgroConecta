import { fireEvent, render, screen, waitFor } from '@testing-library/react-native';
import { useState } from 'react';
import { Pressable, Text, View } from 'react-native';

import { ApiError } from '../api/http';
import type { LoginResponse, Usuario } from '../api/tipos';
import { AuthProvider, useAuth } from './AuthContext';
import type { Sessao } from './sessao';

const mockGetItemAsync = jest.fn();
const mockSetItemAsync = jest.fn();
const mockDeleteItemAsync = jest.fn();

jest.mock('expo-secure-store', () => ({
  getItemAsync: (...args: unknown[]) => mockGetItemAsync(...args),
  setItemAsync: (...args: unknown[]) => mockSetItemAsync(...args),
  deleteItemAsync: (...args: unknown[]) => mockDeleteItemAsync(...args),
}));

const mockAutenticar = jest.fn();
const mockRenovarTokens = jest.fn();
const mockEncerrarSessaoRemota = jest.fn();

jest.mock('../api/auth', () => ({
  autenticar: (...args: unknown[]) => mockAutenticar(...args),
  renovarTokens: (...args: unknown[]) => mockRenovarTokens(...args),
  encerrarSessaoRemota: (...args: unknown[]) => mockEncerrarSessaoRemota(...args),
}));

const USUARIO: Usuario = {
  id: 2,
  nome: 'Paulo Produtor',
  email: 'produtor@agroconecta.local',
  papel: 'Produtor',
  ativo: true,
};

const RESPOSTA: LoginResponse = {
  accessToken: 'access-1',
  refreshToken: 'refresh-1',
  expiresIn: 900,
  usuario: USUARIO,
};

function criarSessao(expiraEm: number): Sessao {
  const agora = Date.now();
  return {
    accessToken: 'access-1',
    refreshToken: 'refresh-1',
    emitidoEm: agora,
    expiraEm,
    usuario: USUARIO,
  };
}

function Harness() {
  const { executarAutenticado, entrar, restaurando, sair, usuario } = useAuth();
  const [resultado, setResultado] = useState('');

  return (
    <View>
      <Text testID="estado">
        {restaurando ? 'restaurando' : usuario ? `logado:${usuario.email}` : 'anonimo'}
      </Text>
      <Text testID="resultado">{resultado}</Text>
      <Pressable
        testID="entrar"
        onPress={() => {
          void entrar('produtor@agroconecta.local', 'Produtor@123');
        }}
      />
      <Pressable
        testID="sair"
        onPress={() => {
          void sair();
        }}
      />
      <Pressable
        testID="executar"
        onPress={() => {
          void executarAutenticado(async (token) => {
            if (token === 'access-1') {
              throw new ApiError(401, 'Token expirado.');
            }
            return 'renovado';
          })
            .then(setResultado)
            .catch(() => setResultado('erro'));
        }}
      />
    </View>
  );
}

async function renderizar() {
  await render(
    <AuthProvider>
      <Harness />
    </AuthProvider>,
  );
}

describe('AuthProvider', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockGetItemAsync.mockResolvedValue(null);
    mockSetItemAsync.mockResolvedValue(undefined);
    mockDeleteItemAsync.mockResolvedValue(undefined);
    mockEncerrarSessaoRemota.mockResolvedValue(undefined);
  });

  it('restaura uma sessão válida salva no SecureStore', async () => {
    mockGetItemAsync.mockResolvedValue(
      JSON.stringify(criarSessao(Date.now() + 900_000)),
    );

    await renderizar();

    expect(
      await screen.findByText('logado:produtor@agroconecta.local'),
    ).toBeTruthy();
    expect(mockRenovarTokens).not.toHaveBeenCalled();
  });

  it('renova a sessão expirada ao restaurar', async () => {
    mockGetItemAsync.mockResolvedValue(JSON.stringify(criarSessao(Date.now() - 1_000)));
    mockRenovarTokens.mockResolvedValue({
      ...RESPOSTA,
      accessToken: 'access-2',
      refreshToken: 'refresh-2',
    });

    await renderizar();

    expect(
      await screen.findByText('logado:produtor@agroconecta.local'),
    ).toBeTruthy();
    expect(mockRenovarTokens).toHaveBeenCalledWith('refresh-1');
    await waitFor(() => expect(mockSetItemAsync).toHaveBeenCalled());
  });

  it('limpa a sessão quando o refresh retorna 401', async () => {
    mockGetItemAsync.mockResolvedValue(JSON.stringify(criarSessao(Date.now() - 1_000)));
    mockRenovarTokens.mockRejectedValue(new ApiError(401, 'Refresh inválido.'));

    await renderizar();

    expect(await screen.findByText('anonimo')).toBeTruthy();
    expect(mockDeleteItemAsync).toHaveBeenCalled();
  });

  it('faz login, persiste a sessão e autentica o usuário', async () => {
    mockAutenticar.mockResolvedValue(RESPOSTA);

    await renderizar();
    await screen.findByText('anonimo');

    await fireEvent.press(screen.getByTestId('entrar'));

    expect(
      await screen.findByText('logado:produtor@agroconecta.local'),
    ).toBeTruthy();
    expect(mockAutenticar).toHaveBeenCalledWith(
      'produtor@agroconecta.local',
      'Produtor@123',
    );
    expect(mockSetItemAsync).toHaveBeenCalled();
  });

  it('encerra a sessão remota e limpa o armazenamento no logout', async () => {
    mockGetItemAsync.mockResolvedValue(
      JSON.stringify(criarSessao(Date.now() + 900_000)),
    );

    await renderizar();
    await screen.findByText('logado:produtor@agroconecta.local');

    await fireEvent.press(screen.getByTestId('sair'));

    expect(await screen.findByText('anonimo')).toBeTruthy();
    expect(mockEncerrarSessaoRemota).toHaveBeenCalledWith('access-1');
    expect(mockDeleteItemAsync).toHaveBeenCalled();
  });

  it('renova o token e repete a operação após um 401', async () => {
    mockGetItemAsync.mockResolvedValue(
      JSON.stringify(criarSessao(Date.now() + 900_000)),
    );
    mockRenovarTokens.mockResolvedValue({
      ...RESPOSTA,
      accessToken: 'access-2',
      refreshToken: 'refresh-2',
    });

    await renderizar();
    await screen.findByText('logado:produtor@agroconecta.local');

    await fireEvent.press(screen.getByTestId('executar'));

    expect(await screen.findByText('renovado')).toBeTruthy();
    expect(mockRenovarTokens).toHaveBeenCalledWith('refresh-1');
  });
});
