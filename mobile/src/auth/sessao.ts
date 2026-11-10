import * as SecureStore from 'expo-secure-store';

import type { Usuario } from '../api/tipos';
import type { Papel } from '../types/dominio';

const CHAVE = 'agroconecta.sessao';

const PAPEIS: Papel[] = ['Admin', 'Produtor', 'Comprador', 'Motorista'];

export type Sessao = {
  accessToken: string;
  refreshToken: string;
  emitidoEm: number;
  expiraEm: number;
  usuario: Usuario;
};

function ehUsuario(valor: unknown): valor is Usuario {
  if (typeof valor !== 'object' || valor === null) {
    return false;
  }
  const usuario = valor as Record<string, unknown>;
  return (
    typeof usuario.id === 'number' &&
    typeof usuario.nome === 'string' &&
    typeof usuario.email === 'string' &&
    typeof usuario.ativo === 'boolean' &&
    typeof usuario.papel === 'string' &&
    PAPEIS.includes(usuario.papel as Papel)
  );
}

function ehSessao(valor: unknown): valor is Sessao {
  if (typeof valor !== 'object' || valor === null) {
    return false;
  }
  const sessao = valor as Record<string, unknown>;
  return (
    typeof sessao.accessToken === 'string' &&
    typeof sessao.refreshToken === 'string' &&
    typeof sessao.emitidoEm === 'number' &&
    Number.isFinite(sessao.emitidoEm) &&
    typeof sessao.expiraEm === 'number' &&
    Number.isFinite(sessao.expiraEm) &&
    ehUsuario(sessao.usuario)
  );
}

/** Salva a sessão no armazenamento seguro do sistema. */
export async function salvarSessao(sessao: Sessao): Promise<void> {
  await SecureStore.setItemAsync(CHAVE, JSON.stringify(sessao));
}

/** Lê a sessão salva. Conteúdo inválido é descartado. */
export async function carregarSessao(): Promise<Sessao | null> {
  const conteudo = await SecureStore.getItemAsync(CHAVE);
  if (!conteudo) {
    return null;
  }

  try {
    const valor: unknown = JSON.parse(conteudo);
    if (ehSessao(valor)) {
      return valor;
    }
  } catch {
    // Conteúdo corrompido: limpa abaixo.
  }

  await SecureStore.deleteItemAsync(CHAVE);
  return null;
}

export async function limparSessao(): Promise<void> {
  await SecureStore.deleteItemAsync(CHAVE);
}

export const CHAVE_SESSAO = CHAVE;
