using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Consultas;
using AgroConecta.Application.Relatorios;

namespace AgroConecta.Tests.Api;

public class RelatoriosApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Admin_obtem_vendas_por_produtor_no_periodo()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var vendas = await cliente.GetFromJsonAsync<List<VendaProdutorResumo>>(
            "/api/v1/relatorios/vendas-produtor?inicio=2026-11-01&fim=2026-11-30",
            Json);

        Assert.NotNull(vendas);
        Assert.Equal(2, vendas!.Count);
        Assert.Contains(vendas, venda => venda.Produtor == "Paulo Produtor" && venda.ValorTotal == 425m);
        Assert.Contains(vendas, venda => venda.Produtor == "Maria Quilombola" && venda.ValorTotal == 320m);
    }

    [Fact]
    public async Task Comprador_nao_acessa_relatorio()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await cliente.GetAsync("/api/v1/relatorios/vendas-produtor");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Periodo_invertido_retorna_400()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.GetAsync(
            "/api/v1/relatorios/vendas-produtor?inicio=2026-11-30&fim=2026-11-01");

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Admin_consulta_auditoria()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var logs = await cliente.GetFromJsonAsync<List<LogAcessoResumo>>(
            "/api/v1/auditoria?limite=10",
            Json);

        Assert.NotNull(logs);
    }

    [Fact]
    public async Task Produtor_nao_acessa_auditoria()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.GetAsync("/api/v1/auditoria");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
