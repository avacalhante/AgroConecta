import { Text } from 'react-native';
import { render, screen } from '@testing-library/react-native';

import { Card } from './Card';

describe('Card', () => {
  it('expõe o título como cabeçalho', async () => {
    await render(
      <Card title="Dados da entrega">
        <Text>Conteúdo</Text>
      </Card>,
    );

    expect(screen.getByRole('header', { name: 'Dados da entrega' })).toBeTruthy();
  });

  it('agrupa um item acionável com rótulo único', async () => {
    await render(
      <Card title="Pedido 1" accessibilityLabel="Pedido 1, Em rota" onPress={() => undefined}>
        <Text>Em rota</Text>
      </Card>,
    );

    expect(screen.getByRole('button', { name: 'Pedido 1, Em rota' })).toBeTruthy();
  });
});
