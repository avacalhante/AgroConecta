/**
 * Configuração do cliente HTTP.
 *
 * `EXPO_PUBLIC_API_URL` é lida em tempo de build pelo Expo.
 * Em dispositivo físico, use o IP da máquina (ex.: http://192.168.0.10:5000/api/v1).
 */
const urlPadrao = 'http://localhost:5000/api/v1';

export const apiConfig = {
  baseUrl: process.env.EXPO_PUBLIC_API_URL?.trim() || urlPadrao,
  timeoutMs: 15_000,
} as const;
