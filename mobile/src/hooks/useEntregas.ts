import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useCallback, useState } from 'react';

import { useAuth } from '../auth/AuthContext';
import { listarEntregasLocais } from '../db/entregas';
import { sincronizarEntregas } from '../sync/entregas';
import type { ResultadoSincronizacao } from '../sync/lotes';

/** Lê as entregas do SQLite e envia as alterações de status pendentes. */
export function useEntregas() {
  const { executarAutenticado } = useAuth();
  const queryClient = useQueryClient();
  const [sincronizando, setSincronizando] = useState(false);
  const [offline, setOffline] = useState(false);

  const consulta = useQuery({
    queryKey: ['entregas-locais'],
    queryFn: listarEntregasLocais,
  });

  const sincronizar = useCallback(async (): Promise<ResultadoSincronizacao | null> => {
    setSincronizando(true);
    let resultado: ResultadoSincronizacao | null = null;

    try {
      resultado = await sincronizarEntregas(executarAutenticado);
      setOffline(resultado.offline);
    } catch {
      setOffline(true);
    } finally {
      setSincronizando(false);
      await queryClient.invalidateQueries({ queryKey: ['entregas-locais'] });
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
