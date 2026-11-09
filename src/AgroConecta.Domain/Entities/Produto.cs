using AgroConecta.Domain.Common;

namespace AgroConecta.Domain.Entities;

public class Produto : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string Unidade { get; set; } = string.Empty;

    public decimal PrecoBase { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<LoteColheita> Lotes { get; set; } = new List<LoteColheita>();

    public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
}
