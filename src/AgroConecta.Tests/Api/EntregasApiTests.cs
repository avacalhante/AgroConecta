using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroConecta.Application.Entregas;

namespace AgroConecta.Tests.Api;

public class EntregasApiTests(ApiFactory fabrica) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task Motorista_lista_as_proprias_entregas()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "motorista@agroconecta.local",
            "Motorista@123");

        var entregas = await cliente.GetFromJsonAsync<List<EntregaDto>>(
            "/api/v1/entregas/minhas",
            Json);

        Assert.NotNull(entregas);
        Assert.True(entregas!.Count >= 2);
        Assert.All(entregas, entrega => Assert.True(entrega.PedidoId > 0));
    }

    [Fact]
    public async Task Comprador_nao_acessa_entregas_do_motorista()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        var resposta = await cliente.GetAsync("/api/v1/entregas/minhas");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Motorista_atualiza_status_da_entrega()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "motorista@agroconecta.local",
            "Motorista@123");

        var resposta = await cliente.PatchAsJsonAsync(
            "/api/v1/entregas/2/status",
            new { status = "EmRota", ocorridoEm = "2026-11-05T08:00:00Z" },
            Json);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var entrega = await resposta.Content.ReadFromJsonAsync<EntregaDto>(Json);
        Assert.Equal("EmRota", entrega!.Status.ToString());
    }

    [Fact]
    public async Task Evidencia_em_multipart_registra_foto_e_coordenadas()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "motorista@agroconecta.local",
            "Motorista@123");

        using var conteudo = new MultipartFormDataContent();
        var arquivo = new ByteArrayContent(Encoding.UTF8.GetBytes("conteudo-de-imagem"));
        arquivo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        conteudo.Add(arquivo, "file", "foto.png");
        conteudo.Add(new StringContent("-22.9068"), "latitude");
        conteudo.Add(new StringContent("-43.1729"), "longitude");
        conteudo.Add(new StringContent("Entregue na portaria"), "observacao");

        var resposta = await cliente.PutAsync("/api/v1/entregas/1/evidencia", conteudo);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var entrega = await resposta.Content.ReadFromJsonAsync<EntregaDto>(Json);
        Assert.NotNull(entrega);
        Assert.StartsWith("/uploads/entregas/", entrega!.FotoUrl);
        Assert.Equal(-22.9068, entrega.Latitude);
        Assert.Equal(-43.1729, entrega.Longitude);
        Assert.Equal("Entregue na portaria", entrega.Observacao);
    }

    [Fact]
    public async Task Evidencia_de_outro_motorista_e_bloqueada()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "comprador@agroconecta.local",
            "Comprador@123");

        using var conteudo = new MultipartFormDataContent();
        var arquivo = new ByteArrayContent(Encoding.UTF8.GetBytes("x"));
        arquivo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        conteudo.Add(arquivo, "file", "foto.png");
        conteudo.Add(new StringContent("0"), "latitude");
        conteudo.Add(new StringContent("0"), "longitude");

        var resposta = await cliente.PutAsync("/api/v1/entregas/1/evidencia", conteudo);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Status_sem_ocorrido_em_retorna_400()
    {
        var cliente = fabrica.CreateAuthenticatedClient(
            "motorista@agroconecta.local",
            "Motorista@123");

        var resposta = await cliente.PatchAsJsonAsync(
            "/api/v1/entregas/2/status",
            new { status = "Entregue" },
            Json);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }
}
