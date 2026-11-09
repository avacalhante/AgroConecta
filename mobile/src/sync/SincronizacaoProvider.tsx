import NetInfo from '@react-native-community/netinfo';
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
import { AppState } from 'react-native';

import { useAuth } from '../auth/AuthContext';
import { contarConflitosSincronizacao } from '../db/estadoSync';
import { sincronizarEntregas } from './entregas';
import { contarItensFila } from './fila';
import { sincronizarLotes, type ResultadoSincronizacao } from './lotes';

const INTERVALO_VERIFICACAO_MS = 30_000;

export type SincronizacaoContextValue = {
  online: boolean | null;
  sincronizando: boolean;
  pendentes: number;
  conflitos: number;
  ultimaSincronizacao: Date | null;
  ultimoResultado: ResultadoSincronizacao | null;
  sincronizarAgora: () => Promise<void>;
};

const SincronizacaoContext = createContext<SincronizacaoContextValue | null>(null);

/** Mantém contadores atualizados e sincroniza ao reconectar ou retomar o app. */
export function SincronizacaoProvider({ children }: PropsWithChildren) {
  const { executarAutenticado, usuario } = useAuth();
  const [online, setOnline] = useState<boolean | null>(null);
  const [sincronizando, setSincronizando] = useState(false);
  const [pendentes, setPendentes] = useState(0);
  const [conflitos, setConflitos] = useState(0);
  const [ultimaSincronizacao, setUltimaSincronizacao] = useState<Date | null>(null);
  const [ultimoResultado, setUltimoResultado] = useState<ResultadoSincronizacao | null>(
    null,
  );
  const execucaoRef = useRef<Promise<void> | null>(null);

  const atualizarContadores = useCallback(async () => {
    try {
      const [totalPendentes, totalConflitos] = await Promise.all([
        contarItensFila(),
        contarConflitosSincronizacao(),
      ]);
      setPendentes(totalPendentes);
      setConflitos(totalConflitos);
    } catch {
      // O banco pode ainda estar abrindo; a próxima execução atualiza.
    }
  }, []);

  const sincronizarAgora = useCallback(async (): Promise<void> => {
    if (!usuario) {
      return;
    }
    if (execucaoRef.current) {
      return execucaoRef.current;
    }

    const execucao = (async () => {
      setSincronizando(true);
      try {
        const lotes = await sincronizarLotes(executarAutenticado);
        const entregas = await sincronizarEntregas(executarAutenticado);
        const combinado: ResultadoSincronizacao = {
          offline: lotes.offline || entregas.offline,
          enviados: lotes.enviados + entregas.enviados,
          conflitos: lotes.conflitos + entregas.conflitos,
          falhas: lotes.falhas + entregas.falhas,
        };

        setOnline(!combinado.offline);
        setUltimoResultado(combinado);
        setUltimaSincronizacao(new Date());
      } catch {
        setOnline(false);
      } finally {
        setSincronizando(false);
        await atualizarContadores();
        execucaoRef.current = null;
      }
    })();

    execucaoRef.current = execucao;
    return execucao;
  }, [atualizarContadores, executarAutenticado, usuario]);

  useEffect(() => {
    if (!usuario) {
      return;
    }

    void sincronizarAgora();
  }, [sincronizarAgora, usuario]);

  useEffect(() => {
    const cancelar = NetInfo.addEventListener((estado) => {
      const conectado =
        estado.isConnected === true && estado.isInternetReachable !== false;
      setOnline(conectado);
      if (conectado) {
        void sincronizarAgora();
      }
    });

    return cancelar;
  }, [sincronizarAgora]);

  useEffect(() => {
    if (!usuario) {
      return;
    }

    const temporizador = setInterval(() => {
      void (async () => {
        try {
          const prontos = await contarItensFila(true);
          if (prontos > 0) {
            await sincronizarAgora();
          }
        } catch {
          // Banco ainda abrindo ou ocupado; o próximo ciclo tenta de novo.
        }
      })();
    }, INTERVALO_VERIFICACAO_MS);

    return () => clearInterval(temporizador);
  }, [sincronizarAgora, usuario]);

  useEffect(() => {
    const inscricao = AppState.addEventListener('change', (estado) => {
      if (estado === 'active') {
        void sincronizarAgora();
      }
    });

    return () => inscricao.remove();
  }, [sincronizarAgora]);

  const valor = useMemo<SincronizacaoContextValue>(
    () => ({
      online,
      sincronizando,
      pendentes: usuario ? pendentes : 0,
      conflitos: usuario ? conflitos : 0,
      ultimaSincronizacao: usuario ? ultimaSincronizacao : null,
      ultimoResultado: usuario ? ultimoResultado : null,
      sincronizarAgora,
    }),
    [
      conflitos,
      online,
      pendentes,
      sincronizando,
      sincronizarAgora,
      ultimaSincronizacao,
      ultimoResultado,
      usuario,
    ],
  );

  return (
    <SincronizacaoContext.Provider value={valor}>
      {children}
    </SincronizacaoContext.Provider>
  );
}

export function useSincronizacao(): SincronizacaoContextValue {
  const contexto = useContext(SincronizacaoContext);
  if (!contexto) {
    throw new Error('useSincronizacao deve ser usado dentro de SincronizacaoProvider.');
  }
  return contexto;
}
