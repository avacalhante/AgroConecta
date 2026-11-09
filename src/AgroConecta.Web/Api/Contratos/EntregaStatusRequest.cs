using AgroConecta.Domain.Enums;

namespace AgroConecta.Web.Api.Contratos;

public record EntregaStatusRequest(
    StatusEntrega Status,
    DateTime OcorridoEm,
    string? Observacao = null);
