using AgroConecta.Domain.Common;

namespace AgroConecta.Domain.Entities;

public class ItemPedido : EntidadeBase
{
    public int PedidoId { get; set; }

    public Pedido Pedido { get; set; } = null!;

    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    public int? LoteId { get; set; }

    public LoteColheita? Lote { get; set; }

    public decimal Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal Subtotal { get; set; }
}
