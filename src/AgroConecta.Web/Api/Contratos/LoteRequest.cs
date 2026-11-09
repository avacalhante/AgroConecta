namespace AgroConecta.Web.Api.Contratos;

/// <summary>Corpo do POST /api/v1/lotes.</summary>
public record LoteRequest(
    int ProdutoId,
    Guid CodigoRastreio,
    DateOnly DataColheita,
    decimal Quantidade);
