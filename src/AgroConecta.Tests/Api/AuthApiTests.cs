using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Auth;

namespace AgroConecta.Tests.Api;

public class AuthApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Login_valido_retorna_tokens_e_usuario()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email = "comprador@agroconecta.local", senha = "Comprador@123" },
            Json);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var login = await resposta.Content.ReadFromJsonAsync<LoginResultado>(Json);

        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));
        Assert.True(login.ExpiresIn > 0);
        Assert.Equal("Carla Compradora", login.Usuario.Nome);
        Assert.Equal("Comprador", login.Usuario.Papel.ToString());
    }

    [Fact]
    public async Task Login_com_senha_errada_retorna_401_em_problem_details()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email = "comprador@agroconecta.local", senha = "SenhaErrada@123" },
            Json);
        var corpo = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        Assert.Equal("application/problem+json", resposta.Content.Headers.ContentType?.MediaType);
        Assert.Contains("/errors/nao-autenticado", corpo);
        Assert.Contains("traceId", corpo);
    }

    [Fact]
    public async Task Login_com_payload_invalido_retorna_400_com_erros()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email = "nao-e-email", senha = "123" },
            Json);
        var corpo = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Contains("/errors/validacao", corpo);
        Assert.Contains("Email", corpo);
        Assert.Contains("Senha", corpo);
    }

    [Fact]
    public async Task Me_sem_token_retorna_401_em_problem_details()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.GetAsync("/api/v1/me");
        var corpo = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        Assert.Equal("application/problem+json", resposta.Content.Headers.ContentType?.MediaType);
        Assert.Contains("/errors/nao-autenticado", corpo);
    }

    [Fact]
    public async Task Me_com_token_retorna_o_usuario_autenticado()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var usuario = await cliente.GetFromJsonAsync<UsuarioDto>("/api/v1/me", Json);

        Assert.NotNull(usuario);
        Assert.Equal("Ana Administradora", usuario!.Nome);
        Assert.Equal("admin@agroconecta.local", usuario.Email);
    }

    [Fact]
    public async Task Refresh_valido_emite_novo_par_de_tokens()
    {
        var cliente = fabrica.CreateClient();
        var login = await LoginAsync(cliente);

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { refreshToken = login.RefreshToken },
            Json);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var renovado = await resposta.Content.ReadFromJsonAsync<LoginResultado>(Json);

        Assert.NotNull(renovado);
        Assert.False(string.IsNullOrWhiteSpace(renovado!.AccessToken));
        Assert.Equal(login.Usuario.Id, renovado.Usuario.Id);
    }

    [Fact]
    public async Task Refresh_com_token_invalido_retorna_401()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { refreshToken = "token-invalido" },
            Json);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Access_token_nao_serve_como_refresh_token()
    {
        var cliente = fabrica.CreateClient();
        var login = await LoginAsync(cliente);

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { refreshToken = login.AccessToken },
            Json);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Logout_autenticado_retorna_204()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.PostAsync("/api/v1/auth/logout", content: null);

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
    }

    [Fact]
    public async Task Produtos_exigem_autenticacao()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.GetAsync("/api/v1/produtos");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    private static async Task<LoginResultado> LoginAsync(HttpClient cliente)
    {
        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email = "admin@agroconecta.local", senha = "Admin@123" },
            Json);

        resposta.EnsureSuccessStatusCode();

        return await resposta.Content.ReadFromJsonAsync<LoginResultado>(Json)
            ?? throw new InvalidOperationException("Login de teste não retornou tokens.");
    }
}
