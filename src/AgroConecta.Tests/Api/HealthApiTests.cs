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
}
