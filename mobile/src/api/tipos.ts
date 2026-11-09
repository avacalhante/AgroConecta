import type { Papel } from '../types/dominio';

/** Formatos de autenticação espelhando `contracts/openapi.yaml`. */

export type Usuario = {
  id: number;
  nome: string;
  email: string;
  papel: Papel;
  ativo: boolean;
};

export type LoginRequest = {
  email: string;
  senha: string;
};

export type LoginResponse = {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  usuario: Usuario;
};

export type RefreshRequest = {
  refreshToken: string;
};

export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
};
