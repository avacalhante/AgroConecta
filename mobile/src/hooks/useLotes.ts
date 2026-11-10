import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useCallback, useState } from 'react';

import { useAuth } from '../auth/AuthContext';
import { listarLotesLocais } from '../db/lotes';
import { sincronizarLotes, type ResultadoSincronizacao } from '../sync/lotes';

/** Lê os lotes do SQLite e expõe a sincronização com a API. */
export function useLotes() {
  const { executarAutenticado } = useAuth();
  const queryClient = useQueryClient();
  const [sincronizando, setSincronizando] = useState(false);
  const [offline, setOffline] = useState(false);

  const consulta = useQuery({
    queryKey: ['lotes-locais'],
    queryFn: listarLotesLocais,
  });

  const sincronizar = useCallback(async (): Promise<ResultadoSincronizacao | null> => {
    setSincronizando(true);
    let resultado: ResultadoSincronizacao | null = null;

    try {
      resultado = await sincronizarLotes(executarAutenticado);
      setOffline(resultado.offline);
    } catch {
      setOffline(true);
    } finally {
      setSincronizando(false);
      await queryClient.invalidateQueries({ queryKey: ['lotes-locais'] });
      await queryClient.invalidateQueries({ queryKey: ['produtos-locais'] });
    }

    return resultado;
  }, [executarAutenticado, queryClient]);

  return {
    ...consulta,
    sincronizar,
    sincronizando,
    offline,
  };
}
