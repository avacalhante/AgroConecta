import type { Lote } from '../api/tipos';
import type { LoteStatus } from '../types/dominio';
import { obterBanco } from './banco';

export type LoteLocal = {
  idLocal: number;
  idRemoto: number | null;
  codigoRastreio: string;
  produtoId: number;
  produtoNome: string;
  produtoUnidade: string;
  dataColheita: string;
  quantidade: number;
  status: LoteStatus;
  criadoEm: string;
  sincronizado: boolean;
  erroSincronizacao: string | null;
};

export type NovoLoteLocal = {
  codigoRastreio: string;
  produtoId: number;
  produtoNome: string;
  produtoUnidade: string;
  dataColheita: string;
  quantidade: number;
  status: LoteStatus;
  criadoEm: string;
};

type LinhaLote = {
  id: number;
  id_remoto: number | null;
  codigo_rastreio: string;
  produto_id: number;
  produto_nome: string;
  produto_unidade: string;
  data_colheita: string;
  quantidade: number;
  status: string;
  criado_em: string;
  sincronizado: number;
  erro_sincronizacao: string | null;
};

function paraLote(linha: LinhaLote): LoteLocal {
  return {
    idLocal: linha.id,
    idRemoto: linha.id_remoto,
    codigoRastreio: linha.codigo_rastreio,
    produtoId: linha.produto_id,
    produtoNome: linha.produto_nome,
    produtoUnidade: linha.produto_unidade,
    dataColheita: linha.data_colheita,
    quantidade: linha.quantidade,
    status: linha.status as LoteStatus,
    criadoEm: linha.criado_em,
    sincronizado: linha.sincronizado === 1,
    erroSincronizacao: linha.erro_sincronizacao,
  };
}

export async function listarLotesLocais(): Promise<LoteLocal[]> {
  const banco = await obterBanco();
  const linhas = await banco.getAllAsync<LinhaLote>(
    'SELECT * FROM lotes ORDER BY criado_em DESC',
  );
  return linhas.map(paraLote);
}

export async function buscarLoteLocal(codigoRastreio: string): Promise<LoteLocal | null> {
  const banco = await obterBanco();
  const linha = await banco.getFirstAsync<LinhaLote>(
    'SELECT * FROM lotes WHERE codigo_rastreio = ?',
    [codigoRastreio],
  );
  return linha ? paraLote(linha) : null;
}

export async function salvarLoteLocal(lote: NovoLoteLocal): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    `INSERT INTO lotes (
       codigo_rastreio, produto_id, produto_nome, produto_unidade,
       data_colheita, quantidade, status, criado_em, sincronizado
     )
     VALUES (?, ?, ?, ?, ?, ?, ?, ?, 0)
     ON CONFLICT(codigo_rastreio) DO UPDATE SET
       produto_id = excluded.produto_id,
       produto_nome = excluded.produto_nome,
       produto_unidade = excluded.produto_unidade,
       data_colheita = excluded.data_colheita,
       quantidade = excluded.quantidade,
       status = excluded.status,
       sincronizado = 0`,
    [
      lote.codigoRastreio,
      lote.produtoId,
      lote.produtoNome,
      lote.produtoUnidade,
      lote.dataColheita,
      lote.quantidade,
      lote.status,
      lote.criadoEm,
    ],
  );
}

export async function marcarLoteSincronizado(
  codigoRastreio: string,
  idRemoto: number | null,
): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    `UPDATE lotes
       SET sincronizado = 1, id_remoto = COALESCE(?, id_remoto), erro_sincronizacao = NULL
     WHERE codigo_rastreio = ?`,
    [idRemoto, codigoRastreio],
  );
}

export async function marcarErroSincronizacaoLote(
  codigoRastreio: string,
  mensagem: string,
): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    'UPDATE lotes SET erro_sincronizacao = ? WHERE codigo_rastreio = ?',
    [mensagem, codigoRastreio],
  );
}

/** Mescla a lista da API no banco local, preservando lotes ainda pendentes. */
export async function upsertLotesRemotos(lotes: Lote[]): Promise<void> {
  if (lotes.length === 0) {
    return;
  }

  const banco = await obterBanco();
  const produtos = await banco.getAllAsync<{ id: number; unidade: string }>(
    'SELECT id, unidade FROM produtos',
  );
  const unidades = new Map(produtos.map((produto) => [produto.id, produto.unidade]));

  for (const lote of lotes) {
    await banco.runAsync(
      `INSERT INTO lotes (
         id_remoto, codigo_rastreio, produto_id, produto_nome, produto_unidade,
         data_colheita, quantidade, status, criado_em, sincronizado
       )
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, 1)
       ON CONFLICT(codigo_rastreio) DO UPDATE SET
         id_remoto = excluded.id_remoto,
         produto_id = excluded.produto_id,
         produto_nome = excluded.produto_nome,
         produto_unidade = excluded.produto_unidade,
         data_colheita = excluded.data_colheita,
         quantidade = excluded.quantidade,
         status = excluded.status,
         sincronizado = 1`,
      [
        lote.id,
        lote.codigoRastreio,
        lote.produtoId,
        lote.produtoNome,
        unidades.get(lote.produtoId) ?? 'kg',
        lote.dataColheita,
        lote.quantidade,
        lote.status,
        lote.criadoEm,
      ],
    );
  }
}
