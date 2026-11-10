/**
 * Com `EXPO_PUBLIC_API_MOCK=true`, os serviços usam o servidor fake em memória.
 * A decisão vale para todos os recursos (auth, lotes, entregas, produtos e indicadores).
 */
export const usarApiFake = process.env.EXPO_PUBLIC_API_MOCK === 'true';
