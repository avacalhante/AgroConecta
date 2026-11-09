namespace AgroConecta.Web.Api.Contratos;

/// <summary>Corpo do POST /api/v1/rotas.</summary>
public record RotaRequest(
    DateOnly Data,
    int MotoristaId,
    int VeiculoId,
    IReadOnlyCollection<int> PedidoIds);
