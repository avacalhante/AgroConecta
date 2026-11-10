import { fireEvent, render, screen, waitFor } from '@testing-library/react-native';
import mockSafeAreaContext from 'react-native-safe-area-context/jest/mock';

import { ApiError } from '../api/http';
import { LoginScreen } from './LoginScreen';

jest.mock('react-native-safe-area-context', () => mockSafeAreaContext);

const mockEntrar = jest.fn();

jest.mock('../auth/AuthContext', () => ({
  useAuth: () => ({ entrar: mockEntrar }),
}));

describe('LoginScreen', () => {
  beforeEach(() => {
    mockEntrar.mockReset();
    mockEntrar.mockResolvedValue(undefined);
  });

  it('mantém o botão desabilitado sem credenciais válidas', async () => {
    await render(<LoginScreen />);

    expect(screen.getByTestId('botao-entrar')).toBeDisabled();
  });

  it('habilita o botão e envia as credenciais ao AuthContext', async () => {
    await render(<LoginScreen />);

    await fireEvent.changeText(
      screen.getByTestId('campo-email'),
      'produtor@agroconecta.local',
    );
    await fireEvent.changeText(screen.getByTestId('campo-senha'), 'Produtor@123');

    const botao = screen.getByTestId('botao-entrar');
    expect(botao).toBeEnabled();

    await fireEvent.press(botao);

    await waitFor(() => {
      expect(mockEntrar).toHaveBeenCalledWith('produtor@agroconecta.local', 'Produtor@123');
    });
  });

  it('exibe os campos com rótulos acessíveis', async () => {
    await render(<LoginScreen />);

    expect(screen.getByLabelText('E-mail')).toBeTruthy();
    expect(screen.getByLabelText('Senha')).toBeTruthy();
  });

  it('não envia credenciais quando o botão está desabilitado', async () => {
    await render(<LoginScreen />);

    await fireEvent.press(screen.getByTestId('botao-entrar'));

    expect(mockEntrar).not.toHaveBeenCalled();
  });

  it('mostra mensagem clara quando o login é recusado', async () => {
    mockEntrar.mockRejectedValueOnce(new ApiError(401, 'Não autenticado.'));
    await render(<LoginScreen />);

    await fireEvent.changeText(
      screen.getByTestId('campo-email'),
      'produtor@agroconecta.local',
    );
    await fireEvent.changeText(screen.getByTestId('campo-senha'), 'senhaerrada');
    await fireEvent.press(screen.getByTestId('botao-entrar'));

    expect(await screen.findByText('E-mail ou senha inválidos.')).toBeTruthy();
  });
});
