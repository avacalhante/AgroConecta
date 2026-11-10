import { usarApiFake } from './fonte';
import { requisitar } from './http';
import { criarLoteFake, listarMeusLotesFake, obterLoteFake } from './mock/servidorFake';
import type { Lote, LotePage, LoteRequest } from './tipos';

export function listarMeusLotes(
  token: string,
  page = 1,
  pageSize = 20,
): Promise<LotePage> {
  if (usarApiFake) {
    return listarMeusLotesFake(token, page, pageSize);
  }

  return requisitar<LotePage>(`/lotes/meus?page=${page}&pageSize=${pageSize}`, {
    token,
  });
}

/**
 * O `codigoRastreio` é gerado no cliente. Um 409 significa que o lote já foi
 * sincronizado e deve ser tratado como sucesso pelo chamador.
 */
export function criarLote(token: string, corpo: LoteRequest): Promise<Lote> {
  if (usarApiFake) {
    return criarLoteFake(token, corpo);
  }

  return requisitar<Lote>('/lotes', { metodo: 'POST', corpo, token });
}

export function obterLote(token: string, id: number): Promise<Lote> {
  if (usarApiFake) {
    return obterLoteFake(token, id);
  }

  return requisitar<Lote>(`/lotes/${id}`, { token });
}
