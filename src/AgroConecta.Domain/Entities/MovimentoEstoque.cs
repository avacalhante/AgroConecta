using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

public class MovimentoEstoque : EntidadeBase
{
    public int LoteId { get; set; }

    public LoteColheita Lote { get; set; } = null!;

    public TipoMovimentoEstoque Tipo { get; set; }

    public decimal Quantidade { get; set; }

    public int? PedidoId { get; set; }

    public Pedido? Pedido { get; set; }
}
