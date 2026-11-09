using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Faturas;
using AgroConecta.Application.Pedidos;
using AgroConecta.Application.Rotas;

namespace AgroConecta.Tests.Api;

public class RotasEFaturasApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Admin_programa_rota_e_cria_entrega_do_pedido()
    {
        var pedidoId = await CriarPedidoSemLoteAsync();
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/rotas",
            new
            {
                data = "2026-11-20",
                motoristaId = 1,
                veiculoId = 1,
                pedidoIds = new[] { pedidoId },
            },
            Json);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);

        var rota = await resposta.Content.ReadFromJsonAsync<RotaDto>(Json);

        Assert.NotNull(rota);
        Assert.Equal("Planejada", rota!.Status.ToString());
        Assert.Single(rota.Entregas);
        Assert.Equal(pedidoId, rota.Entregas.Single().PedidoId);
    }

    [Fact]
    public async Task Pedido_ja_vinculado_a_entrega_retorna_422()
    {
        var pedidoId = await CriarPedidoSemLoteAsync();
        var cliente = fabrica.CreateAuthenticatedClient();

        var primeiro = await cliente.PostAsJsonAsync(
            "/api/v1/rotas",
            new
            {
                data = "2026-11-20",
                motoristaId = 1,
                veiculoId = 1,
                pedidoIds = new[] { pedidoId },
            },
            Json);
        Assert.Equal(HttpStatusCode.Created, primeiro.StatusCode);

        var duplicado = await cliente.PostAsJsonAsync(
            "/api/v1/rotas",
            new
            {
                data = "2026-11-21",
                motoristaId = 1,
                veiculoId = 1,
                pedidoIds = new[] { pedidoId },
            },
            Json);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, duplicado.StatusCode);
    }

    [Fact]
    public async Task Produtor_nao_pode_programar_rota()
    {
        var pedidoId = await CriarPedidoSemLoteAsync();
        var cliente = fabrica.CreateAuthenticatedClient(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/rotas",
            new
            {
                data = "2026-11-20",
                motoristaId = 1,
                veiculoId = 1,
                pedidoIds = new[] { pedidoId },
            },
            Json);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Admin_lista_rotas_do_seed()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var rotas = await cliente.GetFromJsonAsync<List<RotaDto>>("/api/v1/rotas", Json);

        Assert.NotNull(rotas);
        Assert.Contains(rotas!, rota => rota.Motorista == "Marcos Motorista");
    }

    [Fact]
    public async Task Gerar_fatura_e_idempotente_por_pedido()
    {
        var pedidoId = await CriarPedidoSemLoteAsync();
        var cliente = fabrica.CreateAuthenticatedClient();

        var primeira = await cliente.PostAsync($"/api/v1/faturas/pedido/{pedidoId}", null);
        Assert.Equal(HttpStatusCode.Created, primeira.StatusCode);

        var fatura = await primeira.Content.ReadFromJsonAsync<FaturaDto>(Json);
        Assert.NotNull(fatura);
        Assert.Equal(pedidoId, fatura!.PedidoId);
        Assert.StartsWith("FAT-", fatura.Numero);

        var segunda = await cliente.PostAsync($"/api/v1/faturas/pedido/{pedidoId}", null);
        Assert.Equal(HttpStatusCode.Created, segunda.StatusCode);

        var repetida = await segunda.Content.ReadFromJsonAsync<FaturaDto>(Json);
        Assert.NotNull(repetida);
        Assert.Equal(fatura.Id, repetida!.Id);
        Assert.Equal(fatura.Numero, repetida.Numero);
    }

    [Fact]
    public async Task Fatura_de_pedido_cancelado_retorna_422()
    {
        var pedidoId = await CriarPedidoSemLoteAsync();
        var cliente = fabrica.CreateAuthenticatedClient();

        var cancelamento = await cliente.PatchAsJsonAsync(
            $"/api/v1/pedidos/{pedidoId}/status",
            new { status = "Cancelado" },
            Json);
        Assert.Equal(HttpStatusCode.OK, cancelamento.StatusCode);

        var resposta = await cliente.PostAsync($"/api/v1/faturas/pedido/{pedidoId}", null);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }

    [Fact]
    public async Task Produtor_nao_acessa_faturas()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "produtor@agroconecta.local",
            "Produtor@123");

        var resposta = await cliente.GetAsync("/api/v1/faturas");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    private async Task<int> CriarPedidoSemLoteAsync()
    {
        var comprador = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await comprador.PostAsJsonAsync(
            "/api/v1/pedidos",
            new
            {
                enderecoEntrega = "Rua dos Testes, 10 — Campinas/SP",
                itens = new[]
                {
                    new { produtoId = 1, quantidade = 1m, precoUnitario = 8.50m },
                },
            },
            Json);

        resposta.EnsureSuccessStatusCode();

        var pedido = await resposta.Content.ReadFromJsonAsync<PedidoDto>(Json);
        return pedido!.Id;
    }
}
