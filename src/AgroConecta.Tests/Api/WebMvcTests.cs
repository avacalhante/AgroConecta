using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace AgroConecta.Tests.Api;

public class WebMvcTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Rota_protegida_redireciona_anonimo_para_login()
    {
        var cliente = fabrica.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        var resposta = await cliente.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, resposta.StatusCode);
        Assert.Contains("/Account/Login", resposta.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Tela_de_login_abre_para_anonimo()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.GetAsync("/Account/Login");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var html = await resposta.Content.ReadAsStringAsync();
        Assert.Contains("AgroConecta", html);
        Assert.Contains("name=\"Email\"", html);
        Assert.Contains("name=\"Senha\"", html);
    }

    [Fact]
    public async Task Respostas_incluem_cabecalhos_de_seguranca()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.GetAsync("/Account/Login");

        Assert.Equal("nosniff", resposta.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", resposta.Headers.GetValues("X-Frame-Options").Single());
        Assert.Contains("default-src 'self'", resposta.Headers.GetValues("Content-Security-Policy").Single());
    }

    [Fact]
    public async Task Pagina_de_privacidade_abre_para_anonimo()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.GetAsync("/Home/Privacidade");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var html = await resposta.Content.ReadAsStringAsync();
        Assert.Contains("LGPD", html);
    }

    [Fact]
    public async Task Login_com_cookie_permite_acessar_o_painel()
    {
        var cliente = await CriarClienteAutenticadoAsync();

        var painel = await cliente.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, painel.StatusCode);

        var html = await painel.Content.ReadAsStringAsync();
        Assert.Contains("Painel administrativo", html);
    }

    [Theory]
    [InlineData("/Produtores")]
    [InlineData("/Cooperativas")]
    [InlineData("/ProdutosWeb")]
    [InlineData("/PedidosWeb")]
    [InlineData("/EntregasWeb")]
    [InlineData("/EntregasWeb/Criar")]
    [InlineData("/Faturas")]
    [InlineData("/Relatorios/VendasPorProdutor")]
    [InlineData("/Inclusao")]
    [InlineData("/Usuarios")]
    [InlineData("/Usuarios/Auditoria")]
    public async Task Admin_acessa_todas_as_telas_web(string caminho)
    {
        var cliente = await CriarClienteAutenticadoAsync();

        var resposta = await cliente.GetAsync(caminho);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Produtor_nao_acessa_tela_de_usuarios()
    {
        var cliente = await CriarClienteAutenticadoAsync(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.GetAsync("/Usuarios");

        Assert.Equal(HttpStatusCode.Redirect, resposta.StatusCode);
        Assert.Contains("/Account/AcessoNegado", resposta.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Produtor_nao_acessa_relatorio_de_vendas()
    {
        var cliente = await CriarClienteAutenticadoAsync(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.GetAsync("/Relatorios/VendasPorProdutor");

        Assert.Equal(HttpStatusCode.Redirect, resposta.StatusCode);
        Assert.Contains("/Account/AcessoNegado", resposta.Headers.Location?.OriginalString);
    }

    private async Task<HttpClient> CriarClienteAutenticadoAsync(
        string email = "admin@agroconecta.local",
        string senha = "Admin@123")
    {
        var cliente = fabrica.CreateClient(
            new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

        var token = await ObterTokenAntiforgeryAsync(cliente, "/Account/Login");

        var resposta = await cliente.PostAsync(
            "/Account/Login",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Email"] = email,
                ["Senha"] = senha,
                ["__RequestVerificationToken"] = token,
            }));

        Assert.Equal(HttpStatusCode.Redirect, resposta.StatusCode);
        return cliente;
    }

    private static async Task<string> ObterTokenAntiforgeryAsync(HttpClient cliente, string caminho)
    {
        var html = await cliente.GetStringAsync(caminho);
        var correspondencia = Regex.Match(
            html,
            "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");

        Assert.True(correspondencia.Success, "Token antiforgery não encontrado no formulário.");
        return correspondencia.Groups[1].Value;
    }
}
