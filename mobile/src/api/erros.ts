import { ApiError } from './http';

/** Traduz erros de login para mensagens claras em pt-BR. */
export function mensagemDeErroLogin(erro: unknown): string {
  if (!(erro instanceof ApiError)) {
    return 'Não foi possível entrar. Tente novamente.';
  }

  if (erro.status === 401) {
    return 'E-mail ou senha inválidos.';
  }

  const mensagensDosCampos = Object.values(erro.errosPorCampo).flat();
  const primeiraMensagem = mensagensDosCampos[0];
  if (primeiraMensagem) {
    return primeiraMensagem;
  }

  if (erro.status >= 500) {
    return 'O servidor está indisponível. Tente novamente em instantes.';
  }

  return erro.message;
}
