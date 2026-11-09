import type { EntregaStatus } from '../../types/dominio';
import { ApiError } from '../http';
import type {
  Entrega,
  EntregaStatusRequest,
  EvidenciaRequest,
  Indicadores,
  Lote,
  LotePage,
  LoteRequest,
  LoginResponse,
  Produto,
  ProdutoPage,
  Usuario,
} from '../tipos';

/**
 * Servidor fake em memória para desenvolvimento sem API.
 * Imita status codes e mensagens do contrato; não substitui os testes de integração.
 */

type UsuarioFake = {
  senha: string;
  usuario: Usuario;
};

const USUARIOS: UsuarioFake[] = [
  {
    senha: 'Admin@123',
    usuario: {
      id: 1,
      nome: 'Ana Administradora',
      email: 'admin@agroconecta.local',
      papel: 'Admin',
      ativo: true,
    },
  },
  {
    senha: 'Produtor@123',
    usuario: {
      id: 2,
      nome: 'Paulo Produtor',
      email: 'produtor@agroconecta.local',
      papel: 'Produtor',
      ativo: true,
    },
  },
  {
    senha: 'Produtor@123',
    usuario: {
      id: 3,
      nome: 'Maria Quilombola',
      email: 'maria@agroconecta.local',
      papel: 'Produtor',
      ativo: true,
    },
  },
  {
    senha: 'Comprador@123',
    usuario: {
      id: 4,
      nome: 'Carla Compradora',
      email: 'comprador@agroconecta.local',
      papel: 'Comprador',
      ativo: true,
    },
  },
  {
    senha: 'Motorista@123',
    usuario: {
      id: 5,
      nome: 'Marcos Motorista',
      email: 'motorista@agroconecta.local',
      papel: 'Motorista',
      ativo: true,
    },
  },
];

const PRODUTOS: Produto[] = [
  { id: 1, nome: 'Tomate italiano', categoria: 'Hortifrúti', unidade: 'kg', precoBase: 8.5, ativo: true },
  { id: 2, nome: 'Alface crespa', categoria: 'Hortifrúti', unidade: 'unidade', precoBase: 3.2, ativo: true },
  { id: 3, nome: 'Milho verde', categoria: 'Grãos', unidade: 'kg', precoBase: 5.4, ativo: true },
];

function criarLotesIniciais(): Lote[] {
  return [
    {
      id: 1,
      produtorId: 2,
      produtoId: 1,
      produtoNome: 'Tomate italiano',
      codigoRastreio: '550e8400-e29b-41d4-a716-446655440001',
      dataColheita: '2026-11-01',
      quantidade: 120,
      status: 'Disponivel',
      criadoEm: '2026-11-01T10:00:00.000Z',
    },
    {
      id: 2,
      produtorId: 3,
      produtoId: 2,
      produtoNome: 'Alface crespa',
      codigoRastreio: '550e8400-e29b-41d4-a716-446655440002',
      dataColheita: '2026-11-02',
      quantidade: 80,
      status: 'Reservado',
      criadoEm: '2026-11-02T10:00:00.000Z',
    },
  ];
}

function criarEntregasIniciais(): Entrega[] {
  return [
    {
      id: 1,
      pedidoId: 1,
      status: 'EmRota',
      dataPrevista: '2026-11-05',
      dataRealizada: null,
      latitude: null,
      longitude: null,
      fotoUrl: null,
      observacao: 'Entregar na doca 2 até as 14h.',
    },
    {
      id: 2,
      pedidoId: 2,
      status: 'Pendente',
      dataPrevista: '2026-11-06',
      dataRealizada: null,
      latitude: null,
      longitude: null,
      fotoUrl: null,
      observacao: null,
    },
  ];
}

let lotes = criarLotesIniciais();
let entregas = criarEntregasIniciais();

function copiar<T>(valor: T): T {
  return { ...valor };
}

function idDoToken(token: string, prefixo: 'access' | 'refresh'): number | null {
  const marcador = `mock-${prefixo}-`;
  if (!token.startsWith(marcador)) {
    return null;
  }
  const id = Number(token.slice(marcador.length).split('-')[0]);
  return Number.isInteger(id) && id > 0 ? id : null;
}

function buscarUsuario(token: string): Usuario {
  const id = idDoToken(token, 'access');
  const encontrado =
    id === null ? undefined : USUARIOS.find((item) => item.usuario.id === id);
  if (!encontrado) {
    throw new ApiError(401, 'Token inválido.');
  }
  return encontrado.usuario;
}

function criarRespostaLogin(usuario: Usuario): LoginResponse {
  const marca = Date.now();
  return {
    accessToken: `mock-access-${usuario.id}-${marca}`,
    refreshToken: `mock-refresh-${usuario.id}-${marca}`,
    expiresIn: 900,
    usuario,
  };
}

/** Restaura os dados iniciais; usado pelos testes. */
export function reiniciarServidorFake(): void {
  lotes = criarLotesIniciais();
  entregas = criarEntregasIniciais();
}

export async function autenticarFake(email: string, senha: string): Promise<LoginResponse> {
  const encontrado = USUARIOS.find((item) => item.usuario.email === email);
  if (!encontrado || encontrado.senha !== senha) {
    throw new ApiError(401, 'E-mail ou senha inválidos.');
  }
  return criarRespostaLogin(encontrado.usuario);
}

export async function renovarTokensFake(refreshToken: string): Promise<LoginResponse> {
  const id = idDoToken(refreshToken, 'refresh');
  const encontrado =
    id === null ? undefined : USUARIOS.find((item) => item.usuario.id === id);
  if (!encontrado) {
    throw new ApiError(401, 'Refresh token inválido.');
  }
  return criarRespostaLogin(encontrado.usuario);
}

export async function encerrarSessaoRemotaFake(): Promise<void> {
  return undefined;
}

export async function listarProdutosFake(
  token: string,
  page = 1,
  pageSize = 20,
): Promise<ProdutoPage> {
  buscarUsuario(token);
  const inicio = (page - 1) * pageSize;
  return {
    items: PRODUTOS.slice(inicio, inicio + pageSize).map(copiar),
    page,
    pageSize,
    total: PRODUTOS.length,
  };
}

export async function listarMeusLotesFake(
  token: string,
  page = 1,
  pageSize = 20,
): Promise<LotePage> {
  const usuario = buscarUsuario(token);
  const meus =
    usuario.papel === 'Admin'
      ? lotes
      : lotes.filter((lote) => lote.produtorId === usuario.id);
  const inicio = (page - 1) * pageSize;

  return {
    items: meus.slice(inicio, inicio + pageSize).map(copiar),
    page,
    pageSize,
    total: meus.length,
  };
}

export async function criarLoteFake(token: string, corpo: LoteRequest): Promise<Lote> {
  const usuario = buscarUsuario(token);
  if (usuario.papel !== 'Produtor' && usuario.papel !== 'Admin') {
    throw new ApiError(403, 'Apenas produtores registram lotes.');
  }

  if (lotes.some((lote) => lote.codigoRastreio === corpo.codigoRastreio)) {
    throw new ApiError(409, 'Código de rastreio já existente.');
  }

  const produto = PRODUTOS.find((item) => item.id === corpo.produtoId);
  if (!produto) {
    throw new ApiError(422, 'Produto inexistente.');
  }

  if (!Number.isFinite(corpo.quantidade) || corpo.quantidade <= 0) {
    throw new ApiError(422, 'A quantidade deve ser maior que zero.');
  }

  const novo: Lote = {
    id: Math.max(0, ...lotes.map((lote) => lote.id)) + 1,
    produtorId: usuario.id,
    produtoId: produto.id,
    produtoNome: produto.nome,
    codigoRastreio: corpo.codigoRastreio,
    dataColheita: corpo.dataColheita,
    quantidade: corpo.quantidade,
    status: 'Disponivel',
    criadoEm: new Date().toISOString(),
  };

  lotes = [novo, ...lotes];
  return copiar(novo);
}

export async function obterLoteFake(token: string, id: number): Promise<Lote> {
  buscarUsuario(token);
  const lote = lotes.find((item) => item.id === id);
  if (!lote) {
    throw new ApiError(404, 'Lote não encontrado.');
  }
  return copiar(lote);
}

export async function listarMinhasEntregasFake(
  token: string,
  status?: EntregaStatus,
): Promise<Entrega[]> {
  const usuario = buscarUsuario(token);
  if (usuario.papel !== 'Motorista' && usuario.papel !== 'Admin') {
    throw new ApiError(403, 'Sem permissão para listar entregas.');
  }

  return entregas
    .filter((entrega) => !status || entrega.status === status)
    .map(copiar);
}

const TRANSICOES_ENTREGA: Record<EntregaStatus, EntregaStatus[]> = {
  Pendente: ['EmRota', 'Cancelada'],
  EmRota: ['Entregue', 'Falha'],
  Entregue: [],
  Falha: [],
  Cancelada: [],
};

export async function alterarStatusEntregaFake(
  token: string,
  id: number,
  corpo: EntregaStatusRequest,
): Promise<Entrega> {
  buscarUsuario(token);
  const entrega = entregas.find((item) => item.id === id);
  if (!entrega) {
    throw new ApiError(404, 'Entrega não encontrada.');
  }

  const idempotente = entrega.status === corpo.status;
  if (
    !idempotente &&
    !TRANSICOES_ENTREGA[entrega.status].includes(corpo.status)
  ) {
    throw new ApiError(422, 'Transição de status inválida.');
  }

  entrega.status = corpo.status;
  if (corpo.observacao !== undefined) {
    entrega.observacao = corpo.observacao;
  }
  if (corpo.status === 'Entregue') {
    entrega.dataRealizada = corpo.ocorridoEm;
  }

  return copiar(entrega);
}

export async function enviarEvidenciaFake(
  token: string,
  id: number,
  dados: EvidenciaRequest,
): Promise<Entrega> {
  buscarUsuario(token);
  const entrega = entregas.find((item) => item.id === id);
  if (!entrega) {
    throw new ApiError(404, 'Entrega não encontrada.');
  }

  entrega.latitude = dados.latitude;
  entrega.longitude = dados.longitude;
  if (dados.observacao !== undefined) {
    entrega.observacao = dados.observacao;
  }
  if (dados.foto) {
    entrega.fotoUrl = `mock://evidencias/${id}/${Date.now()}`;
  }

  return copiar(entrega);
}

export async function obterIndicadoresFake(token: string): Promise<Indicadores> {
  buscarUsuario(token);
  return {
    pedidosMes: 12,
    entregasNoPrazoPercentual: 92,
    perdasPercentual: 3.5,
    produtoresAtivos: 7,
    cooperativasAtivas: 2,
    entregasComEvidenciaPercentual: 85,
    telasAcessiveisPercentual: 100,
  };
}
