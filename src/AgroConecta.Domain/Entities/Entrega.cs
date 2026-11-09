using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

public class Entrega : EntidadeBase
{
    public int PedidoId { get; set; }

    public Pedido Pedido { get; set; } = null!;

    public int? RotaId { get; set; }

    public Rota? Rota { get; set; }

    public StatusEntrega Status { get; set; } = StatusEntrega.Pendente;

    public DateOnly DataPrevista { get; set; }

    public DateTime? DataRealizada { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? FotoUrl { get; set; }

    public string? Observacao { get; set; }
}
