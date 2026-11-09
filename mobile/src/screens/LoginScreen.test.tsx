import { fireEvent, render, screen } from '@testing-library/react-native';
import mockSafeAreaContext from 'react-native-safe-area-context/jest/mock';

import { LoginScreen } from './LoginScreen';

jest.mock('react-native-safe-area-context', () => mockSafeAreaContext);

describe('LoginScreen', () => {
  it('mantém o botão desabilitado sem credenciais válidas', async () => {
    await render(<LoginScreen onEntrar={jest.fn()} />);

    expect(screen.getByTestId('botao-entrar')).toBeDisabled();
  });

  it('habilita o botão e dispara onEntrar com credenciais preenchidas', async () => {
    const onEntrar = jest.fn();
    await render(<LoginScreen onEntrar={onEntrar} />);

    await fireEvent.changeText(
      screen.getByTestId('campo-email'),
      'produtor@agroconecta.local',
    );
    await fireEvent.changeText(screen.getByTestId('campo-senha'), 'Produtor@123');

    const botao = screen.getByTestId('botao-entrar');
    expect(botao).toBeEnabled();

    await fireEvent.press(botao);
    expect(onEntrar).toHaveBeenCalledTimes(1);
  });

  it('exibe os campos com rótulos acessíveis', async () => {
    await render(<LoginScreen onEntrar={jest.fn()} />);

    expect(screen.getByLabelText('E-mail')).toBeTruthy();
    expect(screen.getByLabelText('Senha')).toBeTruthy();
  });

  it('não dispara onEntrar quando o botão está desabilitado', async () => {
    const onEntrar = jest.fn();
    await render(<LoginScreen onEntrar={onEntrar} />);

    await fireEvent.press(screen.getByTestId('botao-entrar'));

    expect(onEntrar).not.toHaveBeenCalled();
  });
});
