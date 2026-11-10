import { fireEvent, render, screen } from '@testing-library/react-native';

import { Button } from './Button';

describe('Button', () => {
  it('expõe o texto como nome acessível e dispara a ação', async () => {
    const onPress = jest.fn();
    await render(<Button title="Salvar lote" onPress={onPress} />);

    const botao = screen.getByRole('button', { name: 'Salvar lote' });
    await fireEvent.press(botao);

    expect(onPress).toHaveBeenCalledTimes(1);
  });

  it('aceita rótulo acessível diferente do texto visível', async () => {
    await render(
      <Button
        title="Sincronizar agora"
        accessibilityLabel="Sincronizar operações pendentes"
        onPress={() => undefined}
      />,
    );

    expect(
      screen.getByRole('button', { name: 'Sincronizar operações pendentes' }),
    ).toBeTruthy();
  });

  it('anuncia o estado desabilitado e bloqueia o toque', async () => {
    const onPress = jest.fn();
    await render(<Button title="Enviar" onPress={onPress} disabled />);

    const botao = screen.getByRole('button', { name: 'Enviar' });
    expect(botao).toBeDisabled();

    await fireEvent.press(botao);
    expect(onPress).not.toHaveBeenCalled();
  });

  it('anuncia ocupado durante o carregamento', async () => {
    await render(<Button title="Enviando" onPress={() => undefined} loading />);

    const botao = screen.getByRole('button', { name: 'Enviando' });
    expect(botao).toBeDisabled();
    expect(botao.props.accessibilityState).toEqual({ disabled: true, busy: true });
  });
});
