import { fireEvent, render, screen } from '@testing-library/react-native';

import { Field } from './Field';

describe('Field', () => {
  it('associa rótulo, valor e dica ao campo', async () => {
    const onChangeText = jest.fn();
    await render(
      <Field
        label="E-mail"
        value=""
        onChangeText={onChangeText}
        hint="Use o e-mail do seed."
      />,
    );

    const campo = screen.getByLabelText('E-mail');
    await fireEvent.changeText(campo, 'produtor@agroconecta.local');

    expect(onChangeText).toHaveBeenCalledWith('produtor@agroconecta.local');
    expect(campo.props.accessibilityHint).toBe('Use o e-mail do seed.');
  });

  it('mostra o erro e o usa como dica acessível', async () => {
    await render(
      <Field
        label="Senha"
        value=""
        onChangeText={() => undefined}
        error="Informe ao menos 8 caracteres."
      />,
    );

    expect(screen.getByText('Informe ao menos 8 caracteres.')).toBeTruthy();
    expect(screen.getByLabelText('Senha').props.accessibilityHint).toBe(
      'Informe ao menos 8 caracteres.',
    );
  });

  it('identifica campos obrigatórios visualmente', async () => {
    await render(
      <Field label="Quantidade" value="" onChangeText={() => undefined} required />,
    );

    expect(screen.getByText('Quantidade *')).toBeTruthy();
  });
});
