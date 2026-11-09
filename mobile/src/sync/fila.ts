import { obterBanco } from '../db/banco';

export type TipoOperacao = 'criar_lote' | 'atualizar_status_entrega' | 'enviar_evidencia';

export type ItemFila = {
  id: number;
  tipo: TipoOperacao;
  referencia: string;
  payload: string;
  criadoEm: string;
  tentativas: number;
  ultimoErro: string | null;
};

type LinhaFila = {
  id: number;
  tipo: string;
  referencia: string;
  payload: string;
  criado_em: string;
  tentativas: number;
  ultimo_erro: string | null;
};

function paraItem(linha: LinhaFila): ItemFila {
  return {
    id: linha.id,
    tipo: linha.tipo as TipoOperacao,
    referencia: linha.referencia,
    payload: linha.payload,
    criadoEm: linha.criado_em,
    tentativas: linha.tentativas,
    ultimoErro: linha.ultimo_erro,
  };
}

/** Adiciona ou atualiza uma operação na fila, sem duplicar a mesma referência. */
export async function enfileirarOperacao(
  tipo: TipoOperacao,
  referencia: string,
  payload: unknown,
): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    `INSERT INTO fila_sincronizacao (tipo, referencia, payload, criado_em)
     VALUES (?, ?, ?, ?)
     ON CONFLICT(tipo, referencia) DO UPDATE SET
       payload = excluded.payload,
       ultimo_erro = NULL`,
    [tipo, referencia, JSON.stringify(payload), new Date().toISOString()],
  );
}

export async function listarItensFila(tipo?: TipoOperacao): Promise<ItemFila[]> {
  const banco = await obterBanco();
  const linhas = tipo
    ? await banco.getAllAsync<LinhaFila>(
        'SELECT * FROM fila_sincronizacao WHERE tipo = ? ORDER BY id',
        [tipo],
      )
    : await banco.getAllAsync<LinhaFila>(
        'SELECT * FROM fila_sincronizacao ORDER BY id',
      );

  return linhas.map(paraItem);
}

export async function removerItemFila(id: number): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync('DELETE FROM fila_sincronizacao WHERE id = ?', [id]);
}

export async function registrarFalhaItem(id: number, mensagem: string): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync(
    `UPDATE fila_sincronizacao
       SET tentativas = tentativas + 1, ultimo_erro = ?
     WHERE id = ?`,
    [mensagem, id],
  );
}

export async function contarItensFila(): Promise<number> {
  const banco = await obterBanco();
  const linha = await banco.getFirstAsync<{ total: number }>(
    'SELECT COUNT(*) AS total FROM fila_sincronizacao',
  );
  return linha?.total ?? 0;
}
