using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Domain.Entities;

public class Entrega : EntidadeBase
{
    private static readonly IReadOnlyDictionary<StatusEntrega, StatusEntrega[]> TransicoesPermitidas =
        new Dictionary<StatusEntrega, StatusEntrega[]>
        {
            [StatusEntrega.Pendente] = [StatusEntrega.EmRota, StatusEntrega.Cancelada],
            [StatusEntrega.EmRota] = [StatusEntrega.Entregue, StatusEntrega.Falha],
            [StatusEntrega.Entregue] = [],
            [StatusEntrega.Falha] = [],
            [StatusEntrega.Cancelada] = [],
        };

    public int PedidoId { get; set; }

    public Pedido Pedido { get; set; } = null!;

    public int? RotaId { get; set; }

    public Rota? Rota { get; set; }

    public StatusEntrega Status { get; private set; } = StatusEntrega.Pendente;

    public DateOnly DataPrevista { get; set; }

    public DateTime? DataRealizada { get; private set; }

    public double? Latitude { get; private set; }

    public double? Longitude { get; private set; }

    public string? FotoUrl { get; private set; }

    public string? Observacao { get; private set; }

    /// <summary>Atualiza o status respeitando as transições e a idempotência.</summary>
    public void AlterarStatus(StatusEntrega novoStatus)
    {
        if (novoStatus == Status)
        {
            return;
        }

        if (!TransicoesPermitidas[Status].Contains(novoStatus))
        {
            throw new RegraNegocioException(
                $"Transição de {Status} para {novoStatus} não permitida.");
        }

        Status = novoStatus;

        if (novoStatus == StatusEntrega.Entregue)
        {
            DataRealizada ??= DateTime.UtcNow;
        }
    }

    /// <summary>Registra a evidência (foto e coordenadas) da entrega.</summary>
    public void RegistrarEvidencia(
        double latitude,
        double longitude,
        string fotoUrl,
        string? observacao = null)
    {
        if (latitude is < -90 or > 90)
        {
            throw new RegraNegocioException("A latitude deve estar entre -90 e 90.");
        }

        if (longitude is < -180 or > 180)
        {
            throw new RegraNegocioException("A longitude deve estar entre -180 e 180.");
        }

        if (string.IsNullOrWhiteSpace(fotoUrl))
        {
            throw new RegraNegocioException("A foto da entrega é obrigatória.");
        }

        if (Status == StatusEntrega.Cancelada)
        {
            throw new RegraNegocioException("Não é possível registrar evidência de entrega cancelada.");
        }

        Latitude = latitude;
        Longitude = longitude;
        FotoUrl = fotoUrl;
        Observacao = observacao;
    }

    public bool PermiteTransicaoPara(StatusEntrega novoStatus) =>
        novoStatus == Status || TransicoesPermitidas[Status].Contains(novoStatus);
}
