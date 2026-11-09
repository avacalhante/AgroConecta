using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Pedidos;

namespace AgroConecta.Tests.Api;

public class PedidosApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Comprador_lista_apenas_os_proprios_pedidos()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var pagina = await cliente.GetFromJsonAsync<PaginaTeste>("/api/v1/pedidos", Json);

        Assert.NotNull(pagina);
        Assert.True(pagina!.Total >= 2);
        Assert.Single(pagina.Items.Select(pedido => pedido.CompradorId).Distinct());
    }

    [Fact]
    public async Task Admin_lista_todos_os_pedidos()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var pagina = await cliente.GetFromJsonAsync<PaginaTeste>("/api/v1/pedidos", Json);

        Assert.NotNull(pagina);
        Assert.True(pagina!.Total >= 2);
    }

    [Fact]
    public async Task Comprador_cria_pedido_e_total_e_calculado()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/pedidos",
            new
            {
                enderecoEntrega = "Rua Nova, 100 — Campinas/SP",
                itens = new[]
                {
                    new { produtoId = 1, loteId = 1, quantidade = 2m, precoUnitario = 8.50m },
                },
            },
            Json);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);

        var pedido = await resposta.Content.ReadFromJsonAsync<PedidoDto>(Json);

        Assert.NotNull(pedido);
        Assert.Equal(17.00m, pedido!.ValorTotal);
        Assert.Equal("AguardandoAprovacao", pedido.Status.ToString());
        Assert.Single(pedido.Itens);
        Assert.StartsWith("/api/v1/pedidos/", resposta.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task Produtor_nao_pode_criar_pedido()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/pedidos",
            new
            {
                enderecoEntrega = "Rua Nova, 100",
                itens = new[] { new { produtoId = 1, quantidade = 1m, precoUnitario = 8.50m } },
            },
            Json);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Pedido_de_outro_comprador_retorna_404()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.GetAsync("/api/v1/pedidos/1");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Admin_altera_status_com_transicao_valida()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.PatchAsJsonAsync(
            "/api/v1/pedidos/2/status",
            new { status = "EmSeparacao" },
            Json);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var pedido = await resposta.Content.ReadFromJsonAsync<PedidoDto>(Json);
        Assert.Equal("EmSeparacao", pedido!.Status.ToString());
    }

    [Fact]
    public async Task Transicao_invalida_retorna_422()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.PatchAsJsonAsync(
            "/api/v1/pedidos/1/status",
            new { status = "Entregue" },
            Json);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    private sealed record PaginaTeste(List<PedidoDto> Items, int Page, int PageSize, int Total);
}
