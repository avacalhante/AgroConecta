import type { EntregaStatus, LoteStatus, Papel } from '../types/dominio';

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

export type Pagina<T> = {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
};

export type Produto = {
  id: number;
  nome: string;
  categoria: string;
  unidade: string;
  precoBase: number;
  ativo: boolean;
};

export type ProdutoPage = Pagina<Produto>;

export type Lote = {
  id: number;
  produtorId: number;
  produtoId: number;
  produtoNome: string;
  codigoRastreio: string;
  dataColheita: string;
  quantidade: number;
  status: LoteStatus;
  criadoEm: string;
};

export type LoteRequest = {
  produtoId: number;
  codigoRastreio: string;
  dataColheita: string;
  quantidade: number;
};

export type LotePage = Pagina<Lote>;

export type Entrega = {
  id: number;
  pedidoId: number;
  status: EntregaStatus;
  dataPrevista: string;
  dataRealizada?: string | null;
  latitude?: number | null;
  longitude?: number | null;
  fotoUrl?: string | null;
  observacao?: string | null;
};

export type EntregaStatusRequest = {
  status: EntregaStatus;
  ocorridoEm: string;
  observacao?: string;
};

export type EvidenciaRequest = {
  latitude: number;
  longitude: number;
  observacao?: string;
  foto?: {
    uri: string;
    nome: string;
    tipo: string;
  };
};

export type Indicadores = {
  pedidosMes: number;
  entregasNoPrazoPercentual: number;
  perdasPercentual: number;
  produtoresAtivos: number;
  cooperativasAtivas: number;
  entregasComEvidenciaPercentual: number;
  telasAcessiveisPercentual: number;
};
