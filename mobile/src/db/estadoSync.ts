import { obterBanco } from './banco';

/** Quantidade de lotes e entregas com erro de sincronização visível. */
export async function contarConflitosSincronizacao(): Promise<number> {
  const banco = await obterBanco();
  const linha = await banco.getFirstAsync<{ total: number }>(
    `SELECT
       (SELECT COUNT(*) FROM lotes WHERE erro_sincronizacao IS NOT NULL) +
       (SELECT COUNT(*) FROM entregas WHERE erro_sincronizacao IS NOT NULL) AS total`,
  );
  return linha?.total ?? 0;
}
