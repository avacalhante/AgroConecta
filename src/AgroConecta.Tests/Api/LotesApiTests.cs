using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Lotes;

namespace AgroConecta.Tests.Api;

public class LotesApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Lista_lotes_do_seed()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var pagina = await cliente.GetFromJsonAsync<PaginaTeste>("/api/v1/lotes", Json);

        Assert.NotNull(pagina);
        Assert.True(pagina!.Total >= 2);
        Assert.Contains(pagina.Items, lote => lote.ProdutoNome == "Tomate italiano");
    }

    [Fact]
    public async Task Busca_lote_por_id()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var lote = await cliente.GetFromJsonAsync<LoteDto>("/api/v1/lotes/1", Json);

        Assert.NotNull(lote);
        Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440001"), lote!.CodigoRastreio);
        Assert.Equal("Disponivel", lote.Status.ToString());
    }

    [Fact]
    public async Task Produtor_cria_lote_e_duplicidade_retorna_409()
    {
        var cliente = fabrica.CreateAuthenticatedClient("produtor@agroconecta.local", "Produtor@123");
        var codigo = Guid.NewGuid();

        var corpo = new
        {
            produtoId = 3,
            codigoRastreio = codigo,
            dataColheita = "2026-11-01",
            quantidade = 12.5m,
        };

        var primeira = await cliente.PostAsJsonAsync("/api/v1/lotes", corpo, Json);
        var segunda = await cliente.PostAsJsonAsync("/api/v1/lotes", corpo, Json);

        Assert.Equal(HttpStatusCode.Created, primeira.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);

        var texto = await segunda.Content.ReadAsStringAsync();
        Assert.Contains("/errors/conflito", texto);
    }

    [Fact]
    public async Task Comprador_nao_pode_criar_lote()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/lotes",
            new
            {
                produtoId = 1,
                codigoRastreio = Guid.NewGuid(),
                dataColheita = "2026-11-01",
                quantidade = 5m,
            },
            Json);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Produtor_ve_apenas_os_proprios_lotes()
    {
        var cliente = fabrica.CreateAuthenticatedClient("produtor@agroconecta.local", "Produtor@123");

        var pagina = await cliente.GetFromJsonAsync<PaginaTeste>("/api/v1/lotes/meus", Json);

        Assert.NotNull(pagina);
        Assert.NotEmpty(pagina!.Items);
        Assert.Single(pagina.Items.Select(lote => lote.ProdutorId).Distinct());
        Assert.DoesNotContain(pagina.Items, lote => lote.ProdutoNome == "Alface crespa");
    }

    [Fact]
    public async Task Comprador_nao_acessa_rota_meus_lotes()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await cliente.GetAsync("/api/v1/lotes/meus");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    private sealed record PaginaTeste(List<LoteDto> Items, int Page, int PageSize, int Total);
}
