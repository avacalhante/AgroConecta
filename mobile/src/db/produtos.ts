import type { Produto } from '../api/tipos';
import { obterBanco } from './banco';

type LinhaProduto = {
  id: number;
  nome: string;
  categoria: string;
  unidade: string;
  preco_base: number;
  ativo: number;
};

function paraProduto(linha: LinhaProduto): Produto {
  return {
    id: linha.id,
    nome: linha.nome,
    categoria: linha.categoria,
    unidade: linha.unidade,
    precoBase: linha.preco_base,
    ativo: linha.ativo === 1,
  };
}

export async function listarProdutosLocais(): Promise<Produto[]> {
  const banco = await obterBanco();
  const linhas = await banco.getAllAsync<LinhaProduto>(
    'SELECT * FROM produtos WHERE ativo = 1 ORDER BY nome',
  );
  return linhas.map(paraProduto);
}

/** Atualiza o catálogo local com os produtos da API. */
export async function salvarProdutosRemotos(produtos: Produto[]): Promise<void> {
  const banco = await obterBanco();

  for (const produto of produtos) {
    await banco.runAsync(
      `INSERT INTO produtos (id, nome, categoria, unidade, preco_base, ativo)
       VALUES (?, ?, ?, ?, ?, ?)
       ON CONFLICT(id) DO UPDATE SET
         nome = excluded.nome,
         categoria = excluded.categoria,
         unidade = excluded.unidade,
         preco_base = excluded.preco_base,
         ativo = excluded.ativo`,
      [
        produto.id,
        produto.nome,
        produto.categoria,
        produto.unidade,
        produto.precoBase,
        produto.ativo ? 1 : 0,
      ],
    );
  }
}
