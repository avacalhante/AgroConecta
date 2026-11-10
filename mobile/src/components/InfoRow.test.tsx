import { render, screen } from '@testing-library/react-native';

import { InfoRow } from './InfoRow';

describe('InfoRow', () => {
  it('lê rótulo e valor como uma única frase', async () => {
    await render(<InfoRow label="Latitude" value="-22.906800" />);

    expect(screen.getByLabelText('Latitude: -22.906800')).toBeTruthy();
  });
});
