using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AgroConecta.Tests.Api;

public class HealthApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Health_retorna_ok_quando_o_banco_responde()
    {
        var cliente = fabrica.CreateClient();

        var resposta = await cliente.GetAsync("/health");
        var corpo = await resposta.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("ok", corpo.GetProperty("status").GetString());
        Assert.False(string.IsNullOrWhiteSpace(corpo.GetProperty("version").GetString()));
    }

    [Fact]
    public async Task Cors_libera_origem_conhecida_do_aplicativo()
    {
        var cliente = fabrica.CreateClient();

        using var requisicao = new HttpRequestMessage(HttpMethod.Get, "/health");
        requisicao.Headers.Add("Origin", "http://localhost:8081");

        var resposta = await cliente.SendAsync(requisicao);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(
            "http://localhost:8081",
            resposta.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    [Fact]
    public async Task Cors_bloqueia_origem_desconhecida()
    {
        var cliente = fabrica.CreateClient();

        using var requisicao = new HttpRequestMessage(HttpMethod.Get, "/health");
        requisicao.Headers.Add("Origin", "http://origem-nao-permitida.example");

        var resposta = await cliente.SendAsync(requisicao);

        Assert.False(resposta.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
