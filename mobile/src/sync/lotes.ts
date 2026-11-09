import { criarLote, listarMeusLotes } from '../api/lotes';
import { ApiError, erroDefinitivo } from '../api/http';
import { listarProdutos } from '../api/produtos';
import type { LoteRequest } from '../api/tipos';
import {
  marcarErroSincronizacaoLote,
  marcarLoteSincronizado,
  salvarLoteLocal,
  upsertLotesRemotos,
  type NovoLoteLocal,
} from '../db/lotes';
import { salvarProdutosRemotos } from '../db/produtos';
import { estaConectado } from './conexao';
import {
  enfileirarOperacao,
  listarItensFila,
  registrarFalhaItem,
  removerItemFila,
} from './fila';
import { MAX_TENTATIVAS } from '../utils/backoff';

export type ExecutarAutenticado = <T>(acao: (accessToken: string) => Promise<T>) => Promise<T>;

export type ResultadoSincronizacao = {
  offline: boolean;
  enviados: number;
  conflitos: number;
  falhas: number;
};

type LotePendentePayload = LoteRequest & {
  produtoNome: string;
};

/** Salva primeiro no SQLite e enfileira o envio; a rede nunca bloqueia o registro. */
export async function salvarLoteOffline(lote: NovoLoteLocal): Promise<void> {
  await salvarLoteLocal(lote);

  const payload: LotePendentePayload = {
    produtoId: lote.produtoId,
    codigoRastreio: lote.codigoRastreio,
    dataColheita: lote.dataColheita,
    quantidade: lote.quantidade,
    produtoNome: lote.produtoNome,
  };

  await enfileirarOperacao('criar_lote', lote.codigoRastreio, payload);
}

function mensagemDoErro(erro: unknown): string {
  return erro instanceof ApiError ? erro.message : 'Falha ao sincronizar o lote.';
}

/**
 * Envia os lotes pendentes e puxa a lista atualizada.
 * `409` significa que o lote já existe no servidor e conta como sincronizado.
 */
export async function sincronizarLotes(
  executarAutenticado: ExecutarAutenticado,
): Promise<ResultadoSincronizacao> {
  const resultado: ResultadoSincronizacao = {
    offline: false,
    enviados: 0,
    conflitos: 0,
    falhas: 0,
  };

  if (!(await estaConectado())) {
    resultado.offline = true;
    return resultado;
  }

  const pendentes = await listarItensFila('criar_lote', true);

  for (const item of pendentes) {
    let payload: LotePendentePayload | null = null;

    try {
      payload = JSON.parse(item.payload) as LotePendentePayload;
      const corpo: LoteRequest = {
        produtoId: payload.produtoId,
        codigoRastreio: payload.codigoRastreio,
        dataColheita: payload.dataColheita,
        quantidade: payload.quantidade,
      };

      const lote = await executarAutenticado((token) => criarLote(token, corpo));
      await marcarLoteSincronizado(corpo.codigoRastreio, lote.id);
      await removerItemFila(item.id);
      resultado.enviados += 1;
    } catch (erro) {
      const mensagem = mensagemDoErro(erro);

      if (erro instanceof ApiError && erro.status === 409) {
        if (payload) {
          await marcarLoteSincronizado(payload.codigoRastreio, null);
        }
        await removerItemFila(item.id);
        resultado.conflitos += 1;
      } else if (erroDefinitivo(erro)) {
        // 400/403/404/422 não devem ser repetidos; o conflito fica no lote local.
        if (payload) {
          await marcarErroSincronizacaoLote(payload.codigoRastreio, mensagem);
        }
        await removerItemFila(item.id);
        resultado.falhas += 1;
      } else if (item.tentativas + 1 >= MAX_TENTATIVAS) {
        // Limite de tentativas: o lote permanece local com o erro visível.
        if (payload) {
          await marcarErroSincronizacaoLote(
            payload.codigoRastreio,
            `Falha após ${MAX_TENTATIVAS} tentativas: ${mensagem}`,
          );
        }
        await removerItemFila(item.id);
        resultado.falhas += 1;
      } else {
        await registrarFalhaItem(item.id, mensagem);
        resultado.falhas += 1;
      }
    }
  }

  try {
    const pagina = await executarAutenticado((token) => listarMeusLotes(token));
    await upsertLotesRemotos(pagina.items);
  } catch {
    resultado.falhas += 1;
  }

  try {
    const produtos = await executarAutenticado((token) => listarProdutos(token));
    await salvarProdutosRemotos(produtos.items);
  } catch {
    // Catálogo local do seed continua disponível offline.
  }

  return resultado;
}
