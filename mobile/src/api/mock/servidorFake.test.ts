import {
  autenticarFake,
  criarLoteFake,
  listarMeusLotesFake,
  reiniciarServidorFake,
  renovarTokensFake,
} from './servidorFake';

describe('servidor fake', () => {
  beforeEach(() => {
    reiniciarServidorFake();
  });

  it('autentica um usuário do seed', async () => {
    const resposta = await autenticarFake('produtor@agroconecta.local', 'Produtor@123');

    expect(resposta.usuario.nome).toBe('Paulo Produtor');
    expect(resposta.usuario.papel).toBe('Produtor');
    expect(resposta.accessToken).toContain('mock-access-2');
    expect(resposta.expiresIn).toBe(900);
  });

  it('recusa senha inválida com 401', async () => {
    await expect(
      autenticarFake('produtor@agroconecta.local', 'senha-errada'),
    ).rejects.toMatchObject({ status: 401 });
  });

  it('renova os tokens a partir do refresh token', async () => {
    const login = await autenticarFake('motorista@agroconecta.local', 'Motorista@123');
    const renovado = await renovarTokensFake(login.refreshToken);

    expect(renovado.usuario.email).toBe('motorista@agroconecta.local');
    expect(renovado.accessToken).toContain('mock-access-5');
  });

  it('lista apenas os lotes do produtor autenticado', async () => {
    const login = await autenticarFake('produtor@agroconecta.local', 'Produtor@123');
    const pagina = await listarMeusLotesFake(login.accessToken);

    expect(pagina.total).toBe(1);
    expect(pagina.items[0]?.produtoNome).toBe('Tomate italiano');
  });

  it('cria lote e trata o código repetido como 409', async () => {
    const login = await autenticarFake('produtor@agroconecta.local', 'Produtor@123');
    const corpo = {
      produtoId: 3,
      codigoRastreio: '550e8400-e29b-41d4-a716-446655440099',
      dataColheita: '2026-11-10',
      quantidade: 25,
    };

    const novo = await criarLoteFake(login.accessToken, corpo);
    expect(novo.produtoNome).toBe('Milho verde');
    expect((await listarMeusLotesFake(login.accessToken)).total).toBe(2);

    await expect(criarLoteFake(login.accessToken, corpo)).rejects.toMatchObject({
      status: 409,
    });
  });
});
