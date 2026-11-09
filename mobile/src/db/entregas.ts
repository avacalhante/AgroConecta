import type { Entrega } from '../api/tipos';
import type { EntregaStatus } from '../types/dominio';
import { obterBanco } from './banco';

export type EntregaLocal = {
  id: number;
  pedidoId: number;
  status: EntregaStatus;
  dataPrevista: string;
  dataRealizada: string | null;
  latitude: number | null;
  longitude: number | null;
  fotoUrl: string | null;
  observacao: string | null;
  pendente: boolean;
  erroSincronizacao: string | null;
};

type LinhaEntrega = {
  id: number;
  pedido_id: number;
  status: string;
  data_prevista: string;
  data_realizada: string | null;
  latitude: number | null;
  longitude: number | null;
  foto_url: string | null;
  observacao: string | null;
  atualizado_em: string;
  pendente: number;
  erro_sincronizacao: string | null;
};

function paraEntrega(linha: LinhaEntrega): EntregaLocal {
  return {
    id: linha.id,
    pedidoId: linha.pedido_id,
    status: linha.status as EntregaStatus,
    dataPrevista: linha.data_prevista,
    dataRealizada: linha.data_realizada,
    latitude: linha.latitude,
    longitude: linha.longitude,
    fotoUrl: linha.foto_url,
    observacao: linha.observacao,
    pendente: linha.pendente === 1,
    erroSincronizacao: linha.erro_sincronizacao,
  };
}

export async function listarEntregasLocais(): Promise<EntregaLocal[]> {
  const banco = await obterBanco();
  const linhas = await banco.getAllAsync<LinhaEntrega>(
    'SELECT * FROM entregas ORDER BY data_prevista, id',
  );
  return linhas.map(paraEntrega);
}

export async function buscarEntregaLocal(id: number): Promise<EntregaLocal | null> {
  const banco = await obterBanco();
  const linha = await banco.getFirstAsync<LinhaEntrega>(
    'SELECT * FROM entregas WHERE id = ?',
    [id],
  );
  return linha ? paraEntrega(linha) : null;
}

/** Grava a versão do servidor e limpa a marcação de pendência. */
export async function salvarEntregaServidor(
  entrega: Entrega,
  preservarErro = false,
): Promise<void> {
  const banco = await obterBanco();
  const expressaoErro = preservarErro ? 'entregas.erro_sincronizacao' : 'NULL';
  await banco.runAsync(
    `INSERT INTO entregas (
       id, pedido_id, status, data_prevista, data_realizada,
       latitude, longitude, foto_url, observacao, atualizado_em,
       pendente, erro_sincronizacao
     )
     VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0, NULL)
     ON CONFLICT(id) DO UPDATE SET
       pedido_id = excluded.pedido_id,
       status = excluded.status,
       data_prevista = excluded.data_prevista,
       data_realizada = excluded.data_realizada,
       latitude = excluded.latitude,
       longitude = excluded.longitude,
       foto_url = excluded.foto_url,
       observacao = excluded.observacao,
       atualizado_em = excluded.atualizado_em,
       pendente = 0,
       erro_sincronizacao = ${expressaoErro}`,
    [
      entrega.id,
      entrega.pedidoId,
      entrega.status,
      entrega.dataPrevista,
      entrega.dataRealizada ?? null,
      entrega.latitude ?? null,
      entrega.longitude ?? null,
      entrega.fotoUrl ?? null,
      entrega.observacao ?? null,
      new Date().toISOString(),
    ],
  );
}

/** Mescla a lista da API, sem sobrescrever entregas com alteração local pendente. */
export async function upsertEntregasRemotas(entregas: Entrega[]): Promise<void> {
  const banco = await obterBanco();

  for (const entrega of entregas) {
    const existente = await banco.getFirstAsync<{ pendente: number }>(
      'SELECT pendente FROM entregas WHERE id = ?',
      [entrega.id],
    );

    if (existente?.pendente === 1) {
      continue;
    }

    await salvarEntregaServidor(entrega, true);
  }
}

/** Aplica a mudança de status localmente antes de enfileirar o PATCH. */
export async function aplicarStatusLocal(
  id: number,
  status: EntregaStatus,
  ocorridoEm: string,
  observacao?: string,
): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    `UPDATE entregas
       SET status = ?,
           data_realizada = ?,
           observacao = COALESCE(?, observacao),
           pendente = 1,
           erro_sincronizacao = NULL,
           atualizado_em = ?
     WHERE id = ?`,
    [
      status,
      status === 'Entregue' ? ocorridoEm : null,
      observacao ?? null,
      new Date().toISOString(),
      id,
    ],
  );
}

export async function marcarErroSincronizacaoEntrega(
  id: number,
  mensagem: string,
): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    'UPDATE entregas SET erro_sincronizacao = ? WHERE id = ?',
    [mensagem, id],
  );
}

/** Reverte a pendência após erro definitivo, mantendo o conflito visível. */
export async function reverterPendenciaComErro(
  id: number,
  mensagem: string,
): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    'UPDATE entregas SET pendente = 0, erro_sincronizacao = ? WHERE id = ?',
    [mensagem, id],
  );
}
