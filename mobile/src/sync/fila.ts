import { obterBanco } from '../db/banco';
import { calcularBackoffMs } from '../utils/backoff';

export type TipoOperacao = 'criar_lote' | 'atualizar_status_entrega' | 'enviar_evidencia';

export type ItemFila = {
  id: number;
  tipo: TipoOperacao;
  referencia: string;
  payload: string;
  criadoEm: string;
  tentativas: number;
  ultimoErro: string | null;
  proximaTentativaEm: string | null;
};

type LinhaFila = {
  id: number;
  tipo: string;
  referencia: string;
  payload: string;
  criado_em: string;
  tentativas: number;
  ultimo_erro: string | null;
  proxima_tentativa_em: string | null;
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
    proximaTentativaEm: linha.proxima_tentativa_em,
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

export async function listarItensFila(
  tipo?: TipoOperacao,
  apenasProntos = false,
): Promise<ItemFila[]> {
  const banco = await obterBanco();
  const condicoes: string[] = [];
  const parametros: (string | number)[] = [];

  if (tipo) {
    condicoes.push('tipo = ?');
    parametros.push(tipo);
  }

  if (apenasProntos) {
    condicoes.push('(proxima_tentativa_em IS NULL OR proxima_tentativa_em <= ?)');
    parametros.push(new Date().toISOString());
  }

  const filtro = condicoes.length > 0 ? ` WHERE ${condicoes.join(' AND ')}` : '';
  const linhas = await banco.getAllAsync<LinhaFila>(
    `SELECT * FROM fila_sincronizacao${filtro} ORDER BY id`,
    parametros,
  );

  return linhas.map(paraItem);
}

export async function removerItemFila(id: number): Promise<void> {
  const banco = await obterBanco();
  await banco.runAsync('DELETE FROM fila_sincronizacao WHERE id = ?', [id]);
}

/** Incrementa a tentativa e agenda a próxima com backoff exponencial. */
export async function registrarFalhaItem(id: number, mensagem: string): Promise<void> {
  const banco = await obterBanco();
  const atual = await banco.getFirstAsync<{ tentativas: number }>(
    'SELECT tentativas FROM fila_sincronizacao WHERE id = ?',
    [id],
  );
  const tentativas = atual?.tentativas ?? 0;
  const proxima = new Date(Date.now() + calcularBackoffMs(tentativas)).toISOString();

  await banco.runAsync(
    `UPDATE fila_sincronizacao
       SET tentativas = tentativas + 1,
           ultimo_erro = ?,
           proxima_tentativa_em = ?
     WHERE id = ?`,
    [mensagem, proxima, id],
  );
}

export async function contarItensFila(apenasProntos = false): Promise<number> {
  const banco = await obterBanco();
  const linha = apenasProntos
    ? await banco.getFirstAsync<{ total: number }>(
        `SELECT COUNT(*) AS total FROM fila_sincronizacao
         WHERE proxima_tentativa_em IS NULL OR proxima_tentativa_em <= ?`,
        [new Date().toISOString()],
      )
    : await banco.getFirstAsync<{ total: number }>(
        'SELECT COUNT(*) AS total FROM fila_sincronizacao',
      );
  return linha?.total ?? 0;
}
