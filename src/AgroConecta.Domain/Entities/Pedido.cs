using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Entities;

public class Pedido : EntidadeBase
{
    public int CompradorId { get; set; }

    public Usuario Comprador { get; set; } = null!;

    public DateTime DataPedido { get; set; }

    public StatusPedido Status { get; set; } = StatusPedido.Rascunho;

    public decimal ValorTotal { get; set; }

    public string EnderecoEntrega { get; set; } = string.Empty;

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

    public Entrega? Entrega { get; set; }

    public Fatura? Fatura { get; set; }

    public ICollection<MovimentoEstoque> Movimentos { get; set; } = new List<MovimentoEstoque>();
}
