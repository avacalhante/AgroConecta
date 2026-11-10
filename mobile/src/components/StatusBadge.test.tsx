import { render, screen } from '@testing-library/react-native';

import { StatusBadge } from './StatusBadge';

describe('StatusBadge', () => {
  it('mostra o rótulo em português e o anuncia ao leitor', async () => {
    await render(<StatusBadge status="EmRota" />);

    expect(screen.getByText('Em rota')).toBeTruthy();
    expect(screen.getByLabelText('Status: Em rota')).toBeTruthy();
  });

  it('traduz status de falha', async () => {
    await render(<StatusBadge status="Falha" />);

    expect(screen.getByLabelText('Status: Falha')).toBeTruthy();
  });
});
