import { usarApiFake } from './fonte';
import { requisitar } from './http';
import {
  autenticarFake,
  encerrarSessaoRemotaFake,
  renovarTokensFake,
} from './mock/servidorFake';
import type {
  LoginRequest,
  LoginResponse,
  RefreshRequest,
  Usuario,
} from './tipos';

/** Endpoints de autenticação do contrato (`/api/v1/auth/*` e `/api/v1/me`). */

export function autenticar(email: string, senha: string): Promise<LoginResponse> {
  if (usarApiFake) {
    return autenticarFake(email, senha);
  }

  const corpo: LoginRequest = { email, senha };
  return requisitar<LoginResponse>('/auth/login', { metodo: 'POST', corpo });
}

export function renovarTokens(refreshToken: string): Promise<LoginResponse> {
  if (usarApiFake) {
    return renovarTokensFake(refreshToken);
  }

  const corpo: RefreshRequest = { refreshToken };
  return requisitar<LoginResponse>('/auth/refresh', { metodo: 'POST', corpo });
}

export function encerrarSessaoRemota(accessToken: string): Promise<void> {
  if (usarApiFake) {
    return encerrarSessaoRemotaFake();
  }

  return requisitar<void>('/auth/logout', { metodo: 'POST', token: accessToken });
}

export function obterUsuarioAtual(accessToken: string): Promise<Usuario> {
  return requisitar<Usuario>('/me', { token: accessToken });
}
