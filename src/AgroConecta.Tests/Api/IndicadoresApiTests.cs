using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AgroConecta.Application.Indicadores;

namespace AgroConecta.Tests.Api;

public class IndicadoresApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Admin_obtem_indicadores_do_seed()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var indicadores = await cliente.GetFromJsonAsync<IndicadoresDto>(
            "/api/v1/indicadores",
            Json);

        Assert.NotNull(indicadores);
        Assert.Equal(2, indicadores!.ProdutoresAtivos);
        Assert.Equal(1, indicadores.CooperativasAtivas);
        Assert.InRange(indicadores.EntregasNoPrazoPercentual, 0, 100);
        Assert.InRange(indicadores.PerdasPercentual, 0, 100);
        Assert.InRange(indicadores.EntregasComEvidenciaPercentual, 0, 100);
        Assert.Equal(100, indicadores.TelasAcessiveisPercentual);
    }

    [Fact]
    public async Task Comprador_nao_acessa_indicadores()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await cliente.GetAsync("/api/v1/indicadores");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
