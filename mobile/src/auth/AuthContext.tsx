import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
  type PropsWithChildren,
} from 'react';

import { autenticar, encerrarSessaoRemota, renovarTokens } from '../api/auth';
import { ApiError } from '../api/http';
import type { LoginResponse, Usuario } from '../api/tipos';
import { carregarSessao, limparSessao, salvarSessao, type Sessao } from './sessao';

/** Renova um pouco antes do vencimento para evitar requisições com token vencido. */
const ANTECEDENCIA_RENOVACAO = 0.75;
const MARGEM_EXPIRACAO_MS = 30_000;
const ATRASO_MINIMO_RENOVACAO_MS = 5_000;

export type AuthContextValue = {
  usuario: Usuario | null;
  restaurando: boolean;
  saindo: boolean;
  entrar: (email: string, senha: string) => Promise<void>;
  sair: () => Promise<void>;
  /** Tenta renovar o par de tokens; em 401 encerra a sessão local. */
  renovarSessao: () => Promise<boolean>;
  /**
   * Executa uma operação autenticada. Em 401, renova os tokens e repete a
   * operação uma vez; se a renovação falhar, a sessão é encerrada.
   */
  executarAutenticado: <T>(acao: (accessToken: string) => Promise<T>) => Promise<T>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: PropsWithChildren) {
  const [sessao, setSessao] = useState<Sessao | null>(null);
  const [restaurando, setRestaurando] = useState(true);
  const [saindo, setSaindo] = useState(false);
  const sessaoRef = useRef<Sessao | null>(null);
  const renovacaoRef = useRef<Promise<boolean> | null>(null);

  const persistirSessao = useCallback(async (resposta: LoginResponse) => {
    const agora = Date.now();
    const nova: Sessao = {
      accessToken: resposta.accessToken,
      refreshToken: resposta.refreshToken,
      emitidoEm: agora,
      expiraEm: agora + resposta.expiresIn * 1000,
      usuario: resposta.usuario,
    };

    sessaoRef.current = nova;
    setSessao(nova);

    try {
      await salvarSessao(nova);
    } catch {
      // Sem armazenamento seguro disponível, a sessão continua apenas em memória.
    }
  }, []);

  const encerrarSessaoLocal = useCallback(async () => {
    sessaoRef.current = null;
    setSessao(null);
    try {
      await limparSessao();
    } catch {
      // O estado em memória já foi limpo.
    }
  }, []);

  const renovarSessao = useCallback(async (): Promise<boolean> => {
    if (renovacaoRef.current) {
      return renovacaoRef.current;
    }

    const atual = sessaoRef.current;
    if (!atual) {
      return false;
    }

    const promessa = (async () => {
      try {
        const resposta = await renovarTokens(atual.refreshToken);
        await persistirSessao(resposta);
        return true;
      } catch (erro) {
        if (erro instanceof ApiError && erro.status === 401) {
          await encerrarSessaoLocal();
        }
        return false;
      } finally {
        renovacaoRef.current = null;
      }
    })();

    renovacaoRef.current = promessa;
    return promessa;
  }, [encerrarSessaoLocal, persistirSessao]);

  const entrar = useCallback(
    async (email: string, senha: string) => {
      const resposta = await autenticar(email.trim(), senha);
      await persistirSessao(resposta);
    },
    [persistirSessao],
  );

  const sair = useCallback(async () => {
    setSaindo(true);
    try {
      const atual = sessaoRef.current;
      if (atual) {
        try {
          await encerrarSessaoRemota(atual.accessToken);
        } catch {
          // O logout local deve acontecer mesmo se o servidor falhar.
        }
      }
    } finally {
      await encerrarSessaoLocal();
      setSaindo(false);
    }
  }, [encerrarSessaoLocal]);

  const executarAutenticado = useCallback(
    async <T,>(acao: (accessToken: string) => Promise<T>): Promise<T> => {
      const atual = sessaoRef.current;
      if (!atual) {
        throw new ApiError(401, 'Sessão não encontrada. Entre novamente.');
      }

      try {
        return await acao(atual.accessToken);
      } catch (erro) {
        if (!(erro instanceof ApiError) || erro.status !== 401) {
          throw erro;
        }

        const renovou = await renovarSessao();
        const renovada = sessaoRef.current;
        if (!renovou || !renovada) {
          throw erro;
        }

        return acao(renovada.accessToken);
      }
    },
    [renovarSessao],
  );

  useEffect(() => {
    let ativo = true;

    async function restaurar() {
      try {
        const salva = await carregarSessao();
        if (!ativo || !salva) {
          return;
        }

        if (salva.expiraEm > Date.now() + MARGEM_EXPIRACAO_MS) {
          sessaoRef.current = salva;
          setSessao(salva);
          return;
        }

        try {
          const resposta = await renovarTokens(salva.refreshToken);
          if (ativo) {
            await persistirSessao(resposta);
          }
        } catch (erro) {
          if (!ativo) {
            return;
          }
          if (erro instanceof ApiError && erro.status === 401) {
            await encerrarSessaoLocal();
          } else {
            // Falha de rede ou servidor: mantém a sessão para uso offline.
            sessaoRef.current = salva;
            setSessao(salva);
          }
        }
      } catch {
        if (ativo) {
          await encerrarSessaoLocal();
        }
      } finally {
        if (ativo) {
          setRestaurando(false);
        }
      }
    }

    void restaurar();

    return () => {
      ativo = false;
    };
  }, [encerrarSessaoLocal, persistirSessao]);

  useEffect(() => {
    if (!sessao) {
      return;
    }

    const duracao = Math.max(sessao.expiraEm - sessao.emitidoEm, 0);
    const alvoRenovacao = sessao.emitidoEm + duracao * ANTECEDENCIA_RENOVACAO;
    const atraso = Math.max(alvoRenovacao - Date.now(), ATRASO_MINIMO_RENOVACAO_MS);

    const temporizador = setTimeout(() => {
      void renovarSessao();
    }, atraso);

    return () => clearTimeout(temporizador);
  }, [renovarSessao, sessao]);

  const valor = useMemo<AuthContextValue>(
    () => ({
      usuario: sessao?.usuario ?? null,
      restaurando,
      saindo,
      entrar,
      sair,
      renovarSessao,
      executarAutenticado,
    }),
    [entrar, executarAutenticado, renovarSessao, restaurando, sair, saindo, sessao],
  );

  return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const contexto = useContext(AuthContext);
  if (!contexto) {
    throw new Error('useAuth deve ser usado dentro de AuthProvider.');
  }
  return contexto;
}
