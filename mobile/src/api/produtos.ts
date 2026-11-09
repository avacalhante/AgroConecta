import { usarApiFake } from './fonte';
import { requisitar } from './http';
import { listarProdutosFake } from './mock/servidorFake';
import type { ProdutoPage } from './tipos';

export function listarProdutos(
  token: string,
  page = 1,
  pageSize = 20,
): Promise<ProdutoPage> {
  if (usarApiFake) {
    return listarProdutosFake(token, page, pageSize);
  }

  return requisitar<ProdutoPage>(
    `/produtos?page=${page}&pageSize=${pageSize}&ativo=true`,
    { token },
  );
}
