import { ApiError, requisitar } from './http';

type RespostaFake = {
  ok: boolean;
  status: number;
  headers: { get: (nome: string) => string | null };
  json: () => Promise<unknown>;
};

function criarResposta(
  status: number,
  corpo?: unknown,
  tipo = 'application/json',
): RespostaFake {
  return {
    ok: status >= 200 && status < 300,
    status,
    headers: { get: () => tipo },
    json: async () => corpo,
  };
}

function instalarFetch(fetchFake: jest.Mock): void {
  globalThis.fetch = fetchFake as unknown as typeof fetch;
}

async function capturarErro(promessa: Promise<unknown>): Promise<ApiError> {
  try {
    await promessa;
  } catch (erro) {
    if (erro instanceof ApiError) {
      return erro;
    }
    throw erro;
  }
  throw new Error('A requisição deveria ter falhado.');
}

const fetchOriginal = globalThis.fetch;

describe('requisitar', () => {
  afterAll(() => {
    globalThis.fetch = fetchOriginal;
  });

  it('envia o token e devolve o JSON da resposta', async () => {
    const fetchFake = jest.fn().mockResolvedValue(criarResposta(200, { id: 10 }));
    instalarFetch(fetchFake);

    const resultado = await requisitar<{ id: number }>('/lotes/10', { token: 'abc' });

    expect(resultado).toEqual({ id: 10 });
    const [url, opcoes] = fetchFake.mock.calls[0] as [string, RequestInit];
    expect(url).toContain('/api/v1/lotes/10');
    expect(opcoes.headers).toMatchObject({ Authorization: 'Bearer abc' });
  });

  it('converte 400 com erros por campo', async () => {
    instalarFetch(
      jest.fn().mockResolvedValue(
        criarResposta(400, {
          status: 400,
          detail: 'Um ou mais campos são inválidos.',
          errors: { email: ['E-mail inválido.'] },
        }),
      ),
    );

    const erro = await capturarErro(requisitar('/auth/login', { metodo: 'POST' }));

    expect(erro.status).toBe(400);
    expect(erro.message).toBe('Um ou mais campos são inválidos.');
    expect(erro.errosPorCampo).toEqual({ email: ['E-mail inválido.'] });
  });

  it('converte 422 em erro de regra de negócio', async () => {
    instalarFetch(
      jest.fn().mockResolvedValue(
        criarResposta(422, {
          status: 422,
          detail: 'A quantidade deve ser maior que zero.',
        }),
      ),
    );

    const erro = await capturarErro(requisitar('/lotes', { metodo: 'POST' }));

    expect(erro.status).toBe(422);
    expect(erro.message).toBe('A quantidade deve ser maior que zero.');
  });

  it('converte 409 de lote duplicado', async () => {
    instalarFetch(
      jest.fn().mockResolvedValue(
        criarResposta(409, {
          status: 409,
          detail: 'Código de rastreio já existente.',
        }),
      ),
    );

    const erro = await capturarErro(requisitar('/lotes', { metodo: 'POST' }));

    expect(erro.status).toBe(409);
  });

  it('converte falha de rede em status 0', async () => {
    instalarFetch(jest.fn().mockRejectedValue(new TypeError('Network request failed')));

    const erro = await capturarErro(requisitar('/indicadores'));

    expect(erro.status).toBe(0);
    expect(erro.message).toContain('Sem conexão');
  });

  it('devolve undefined para respostas 204', async () => {
    instalarFetch(jest.fn().mockResolvedValue(criarResposta(204)));

    await expect(
      requisitar('/auth/logout', { metodo: 'POST', token: 'abc' }),
    ).resolves.toBeUndefined();
  });

  it('envia multipart sem definir Content-Type manualmente', async () => {
    const formulario = { append: jest.fn() } as unknown as FormData;
    const fetchFake = jest.fn().mockResolvedValue(criarResposta(200, { id: 1 }));
    instalarFetch(fetchFake);

    await requisitar('/entregas/1/evidencia', {
      metodo: 'PUT',
      formData: formulario,
      token: 'abc',
    });

    const [, opcoes] = fetchFake.mock.calls[0] as [string, RequestInit];
    expect(opcoes.body).toBe(formulario);
    expect(opcoes.headers).not.toHaveProperty('Content-Type');
  });
});
