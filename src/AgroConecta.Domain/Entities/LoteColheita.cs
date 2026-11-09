using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Domain.Entities;

public class LoteColheita : EntidadeBase
{
    public int ProdutorId { get; set; }

    public Produtor Produtor { get; set; } = null!;

    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    /// <summary>Código gerado no cliente para permitir sincronização idempotente.</summary>
    public Guid CodigoRastreio { get; set; }

    public DateOnly DataColheita { get; set; }

    public decimal Quantidade { get; set; }

    public StatusLote Status { get; private set; } = StatusLote.Disponivel;

    public ICollection<MovimentoEstoque> Movimentos { get; set; } = new List<MovimentoEstoque>();

    public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();

    public void Reservar()
    {
        if (Status != StatusLote.Disponivel)
        {
            throw new RegraNegocioException($"Lote com status {Status} não pode ser reservado.");
        }

        Status = StatusLote.Reservado;
    }

    public void Liberar()
    {
        if (Status != StatusLote.Reservado)
        {
            throw new RegraNegocioException($"Lote com status {Status} não pode ser liberado.");
        }

        Status = StatusLote.Disponivel;
    }

    public void Esgotar()
    {
        if (Status == StatusLote.Esgotado)
        {
            return;
        }

        Status = StatusLote.Esgotado;
    }
}
