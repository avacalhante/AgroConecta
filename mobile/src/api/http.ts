import { apiConfig } from './config';
import type { ProblemDetails } from './tipos';

/** Erro de requisição com status HTTP e corpo RFC 7807 opcional. */
export class ApiError extends Error {
  readonly status: number;
  readonly problema?: ProblemDetails;

  constructor(status: number, mensagem: string, problema?: ProblemDetails) {
    super(mensagem);
    this.name = 'ApiError';
    this.status = status;
    this.problema = problema;
  }

  /** Mensagens por campo retornadas em erros 400. */
  get errosPorCampo(): Record<string, string[]> {
    return this.problema?.errors ?? {};
  }
}

export type MetodoHttp = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export type OpcoesRequisicao = {
  metodo?: MetodoHttp;
  corpo?: unknown;
  token?: string;
  timeoutMs?: number;
};

function urlCompleta(caminho: string): string {
  const base = apiConfig.baseUrl.replace(/\/+$/, '');
  return `${base}${caminho.startsWith('/') ? caminho : `/${caminho}`}`;
}

async function lerProblema(resposta: Response): Promise<ProblemDetails | undefined> {
  const tipo = resposta.headers.get('content-type') ?? '';
  if (!tipo.includes('json')) {
    return undefined;
  }
  try {
    return (await resposta.json()) as ProblemDetails;
  } catch {
    return undefined;
  }
}

function mensagemPadrao(status: number): string {
  switch (status) {
    case 400:
      return 'Dados inválidos. Verifique os campos.';
    case 401:
      return 'Sessão expirada. Entre novamente.';
    case 403:
      return 'Você não tem permissão para esta ação.';
    case 404:
      return 'Recurso não encontrado.';
    case 409:
      return 'Registro já existente.';
    case 422:
      return 'Regra de negócio violada.';
    default:
      return status >= 500
        ? 'O servidor não conseguiu concluir a operação.'
        : 'Não foi possível concluir a requisição.';
  }
}

/**
 * Requisição JSON com timeout e normalização de erros.
 * Status 0 representa falha de rede ou timeout, sem resposta HTTP.
 */
export async function requisitar<T>(
  caminho: string,
  opcoes: OpcoesRequisicao = {},
): Promise<T> {
  const controlador = new AbortController();
  const temporizador = setTimeout(
    () => controlador.abort(),
    opcoes.timeoutMs ?? apiConfig.timeoutMs,
  );
  const temCorpo = opcoes.corpo !== undefined;

  try {
    const resposta = await fetch(urlCompleta(caminho), {
      method: opcoes.metodo ?? 'GET',
      headers: {
        Accept: 'application/json',
        ...(temCorpo ? { 'Content-Type': 'application/json' } : {}),
        ...(opcoes.token ? { Authorization: `Bearer ${opcoes.token}` } : {}),
      },
      body: temCorpo ? JSON.stringify(opcoes.corpo) : undefined,
      signal: controlador.signal,
    });

    if (!resposta.ok) {
      const problema = await lerProblema(resposta);
      throw new ApiError(
        resposta.status,
        problema?.detail ?? problema?.title ?? mensagemPadrao(resposta.status),
        problema,
      );
    }

    if (resposta.status === 204) {
      return undefined as T;
    }

    return (await resposta.json()) as T;
  } catch (erro) {
    if (erro instanceof ApiError) {
      throw erro;
    }
    if (erro instanceof Error && erro.name === 'AbortError') {
      throw new ApiError(0, 'Tempo de conexão esgotado. Verifique a rede e tente novamente.');
    }
    throw new ApiError(0, 'Sem conexão com o servidor. Verifique a rede e tente novamente.');
  } finally {
    clearTimeout(temporizador);
  }
}
