using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Entregas;
using AgroConecta.Application.Faturas;
using AgroConecta.Application.Lotes;
using AgroConecta.Application.Pedidos;
using AgroConecta.Application.Produtos;
using AgroConecta.Application.Rotas;

namespace AgroConecta.Tests.Api;

/// <summary>
/// Exercita o fluxo de ponta a ponta previsto no manual:
/// produtor → produto → lote → pedido → rota → entrega → fatura.
/// </summary>
public class FluxoCompletoApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Fluxo_completo_da_cadeia_produtiva()
    {
        // 1. Admin cadastra um produto no catálogo.
        var admin = fabrica.CreateAuthenticatedClient();

        var respostaProduto = await admin.PostAsJsonAsync(
            "/api/v1/produtos",
            new
            {
                nome = $"Produto E2E {Guid.NewGuid():N}",
                categoria = "Hortifrúti",
                unidade = "kg",
                precoBase = 6.00m,
            },
            Json);

        Assert.Equal(HttpStatusCode.Created, respostaProduto.StatusCode);
        var produto = await respostaProduto.Content.ReadFromJsonAsync<ProdutoDto>(Json);
        Assert.NotNull(produto);

        // 2. Produtor registra o lote da colheita.
        var produtor = fabrica.CreateAuthenticatedClient(
            "produtor@agroconecta.local",
            "Produtor@123");

        var respostaLote = await produtor.PostAsJsonAsync(
            "/api/v1/lotes",
            new
            {
                produtoId = produto!.Id,
                codigoRastreio = Guid.NewGuid(),
                dataColheita = "2026-11-01",
                quantidade = 40m,
            },
            Json);

        Assert.Equal(HttpStatusCode.Created, respostaLote.StatusCode);
        var lote = await respostaLote.Content.ReadFromJsonAsync<LoteDto>(Json);
        Assert.NotNull(lote);

        // 3. Comprador cria o pedido a partir do lote.
        var comprador = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var respostaPedido = await comprador.PostAsJsonAsync(
            "/api/v1/pedidos",
            new
            {
                enderecoEntrega = "Rua do Fluxo, 50 — Campinas/SP",
                itens = new[]
                {
                    new { produtoId = produto.Id, loteId = lote!.Id, quantidade = 10m, precoUnitario = 6.00m },
                },
            },
            Json);

        Assert.Equal(HttpStatusCode.Created, respostaPedido.StatusCode);
        var pedido = await respostaPedido.Content.ReadFromJsonAsync<PedidoDto>(Json);
        Assert.NotNull(pedido);
        Assert.Equal(60.00m, pedido!.ValorTotal);

        // 4. Admin programa a rota, gerando a entrega do pedido.
        var respostaRota = await admin.PostAsJsonAsync(
            "/api/v1/rotas",
            new
            {
                data = "2026-11-20",
                motoristaId = 1,
                veiculoId = 1,
                pedidoIds = new[] { pedido.Id },
            },
            Json);

        Assert.Equal(HttpStatusCode.Created, respostaRota.StatusCode);
        var rota = await respostaRota.Content.ReadFromJsonAsync<RotaDto>(Json);
        Assert.NotNull(rota);
        Assert.Single(rota!.Entregas);
        var entregaId = rota.Entregas.Single().Id;

        // 5. Motorista inicia e conclui a entrega.
        var motorista = fabrica.CreateAuthenticatedClient(
            "motorista@agroconecta.local",
            "Motorista@123");

        var inicio = await motorista.PatchAsJsonAsync(
            $"/api/v1/entregas/{entregaId}/status",
            new { status = "EmRota", ocorridoEm = "2026-11-20T08:00:00Z" },
            Json);
        Assert.Equal(HttpStatusCode.OK, inicio.StatusCode);

        var conclusao = await motorista.PatchAsJsonAsync(
            $"/api/v1/entregas/{entregaId}/status",
            new { status = "Entregue", ocorridoEm = "2026-11-20T15:00:00Z" },
            Json);
        Assert.Equal(HttpStatusCode.OK, conclusao.StatusCode);

        var entrega = await conclusao.Content.ReadFromJsonAsync<EntregaDto>(Json);
        Assert.Equal("Entregue", entrega!.Status.ToString());

        // 6. Admin emite a fatura do pedido.
        var respostaFatura = await admin.PostAsync(
            $"/api/v1/faturas/pedido/{pedido.Id}",
            null);

        Assert.Equal(HttpStatusCode.Created, respostaFatura.StatusCode);
        var fatura = await respostaFatura.Content.ReadFromJsonAsync<FaturaDto>(Json);
        Assert.NotNull(fatura);
        Assert.Equal(pedido.Id, fatura!.PedidoId);
        Assert.Equal(60.00m, fatura.Valor);
        Assert.Equal("Emitida", fatura.Status.ToString());
    }
}
