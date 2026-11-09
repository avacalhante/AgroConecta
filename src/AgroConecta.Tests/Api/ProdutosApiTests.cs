using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AgroConecta.Application.Produtos;

namespace AgroConecta.Tests.Api;

public class ProdutosApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Lista_produtos_paginados_do_seed()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.GetAsync("/api/v1/produtos?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var pagina = await resposta.Content.ReadFromJsonAsync<PaginaTeste>(Json);

        Assert.NotNull(pagina);
        Assert.True(pagina!.Total >= 3);
        Assert.Equal(1, pagina.Page);
        Assert.Contains(pagina.Items, item => item.Nome == "Tomate italiano");
    }

    [Fact]
    public async Task Filtra_produtos_ativos()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var pagina = await cliente.GetFromJsonAsync<PaginaTeste>("/api/v1/produtos?ativo=true", Json);

        Assert.NotNull(pagina);
        Assert.All(pagina!.Items, item => Assert.True(item.Ativo));
    }

    [Fact]
    public async Task Busca_produto_por_id()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var produto = await cliente.GetFromJsonAsync<ProdutoDto>("/api/v1/produtos/1", Json);

        Assert.NotNull(produto);
        Assert.Equal("Tomate italiano", produto!.Nome);
    }

    [Fact]
    public async Task Retorna_404_em_problem_details_para_produto_inexistente()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.GetAsync("/api/v1/produtos/999");
        var corpo = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        Assert.Equal("application/problem+json", resposta.Content.Headers.ContentType?.MediaType);
        Assert.Contains("/errors/nao-encontrado", corpo);
        Assert.Contains("traceId", corpo);
    }

    [Fact]
    public async Task Cria_produto_e_retorna_201()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/produtos",
            new { nome = "Abóbora cabotiá", categoria = "Hortifrúti", unidade = "kg", precoBase = 6.20m },
            Json);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);

        var produto = await resposta.Content.ReadFromJsonAsync<ProdutoDto>(Json);
        Assert.NotNull(produto);
        Assert.Equal("Abóbora cabotiá", produto!.Nome);
        Assert.Equal($"/api/v1/produtos/{produto.Id}", resposta.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task Retorna_400_com_erros_por_campo_em_entrada_invalida()
    {
        var cliente = fabrica.CreateAuthenticatedClient();

        var resposta = await cliente.PostAsJsonAsync(
            "/api/v1/produtos",
            new { nome = "", categoria = "", unidade = "", precoBase = -1m },
            Json);
        var corpo = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Contains("/errors/validacao", corpo);
        Assert.Contains("errors", corpo);
        Assert.Contains("Nome", corpo);
    }

    private sealed record PaginaTeste(List<ProdutoDto> Items, int Page, int PageSize, int Total);
}
