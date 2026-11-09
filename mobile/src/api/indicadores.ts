import { usarApiFake } from './fonte';
import { requisitar } from './http';
import { obterIndicadoresFake } from './mock/servidorFake';
import type { Indicadores } from './tipos';

export function obterIndicadores(token: string): Promise<Indicadores> {
  if (usarApiFake) {
    return obterIndicadoresFake(token);
  }

  return requisitar<Indicadores>('/indicadores', { token });
}
