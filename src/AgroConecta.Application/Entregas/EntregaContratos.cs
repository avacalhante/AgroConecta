using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Entregas;

public record EntregaDto(
    int Id,
    int PedidoId,
    StatusEntrega Status,
    DateOnly DataPrevista,
    DateTime? DataRealizada,
    double? Latitude,
    double? Longitude,
    string? FotoUrl,
    string? Observacao);

public record ListarMinhasEntregasConsulta(int UsuarioId, bool EhAdmin, StatusEntrega? Status = null);

public record AlterarStatusEntregaComando(
    int EntregaId,
    int UsuarioId,
    bool EhAdmin,
    StatusEntrega Status,
    string? Observacao = null);

public record RegistrarEvidenciaComando(
    int EntregaId,
    int UsuarioId,
    bool EhAdmin,
    double Latitude,
    double Longitude,
    string FotoUrl,
    string? Observacao = null);

public static class MapeadorEntrega
{
    public static EntregaDto ParaDto(Entrega entrega) => new(
        entrega.Id,
        entrega.PedidoId,
        entrega.Status,
        entrega.DataPrevista,
        entrega.DataRealizada,
        entrega.Latitude,
        entrega.Longitude,
        entrega.FotoUrl,
        entrega.Observacao);
}
