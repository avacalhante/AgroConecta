using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

public class Fatura : EntidadeBase
{
    public int PedidoId { get; set; }

    public Pedido Pedido { get; set; } = null!;

    public string Numero { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public StatusFatura Status { get; set; } = StatusFatura.Pendente;

    public DateTime? EmitidaEm { get; set; }

    public DateTime? PagoEm { get; set; }
}
