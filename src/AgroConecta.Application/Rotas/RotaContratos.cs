using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Rotas;

public record EntregaRotaDto(
    int Id,
    int PedidoId,
    string Comprador,
    StatusEntrega Status,
    DateOnly DataPrevista);

public record RotaDto(
    int Id,
    DateOnly Data,
    string Motorista,
    string Veiculo,
    StatusRota Status,
    IReadOnlyCollection<EntregaRotaDto> Entregas);

public record CriarRotaComando(
    DateOnly Data,
    int MotoristaId,
    int VeiculoId,
    IReadOnlyCollection<int> PedidoIds);

public static class MapeadorRota
{
    public static RotaDto ParaDto(Rota rota) => new(
        rota.Id,
        rota.Data,
        rota.Motorista?.Usuario?.Nome ?? string.Empty,
        rota.Veiculo is null ? string.Empty : $"{rota.Veiculo.Placa} · {rota.Veiculo.Tipo}",
        rota.Status,
        rota.Entregas
            .Select(entrega => new EntregaRotaDto(
                entrega.Id,
                entrega.PedidoId,
                entrega.Pedido?.Comprador?.Nome ?? string.Empty,
                entrega.Status,
                entrega.DataPrevista))
            .ToArray());
}
