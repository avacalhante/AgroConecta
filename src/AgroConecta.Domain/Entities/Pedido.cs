using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Domain.Entities;

/// <summary>
/// Agregado do pedido B2B. Os itens ficam em lista privada e só entram pelos
/// métodos de negócio, preservando subtotal e total consistentes.
/// </summary>
public class Pedido : EntidadeBase
{
    private static readonly IReadOnlyDictionary<StatusPedido, StatusPedido[]> TransicoesPermitidas =
        new Dictionary<StatusPedido, StatusPedido[]>
        {
            [StatusPedido.Rascunho] = [StatusPedido.AguardandoAprovacao, StatusPedido.Cancelado],
            [StatusPedido.AguardandoAprovacao] = [StatusPedido.Aprovado, StatusPedido.Cancelado],
            [StatusPedido.Aprovado] = [StatusPedido.EmSeparacao, StatusPedido.Cancelado],
            [StatusPedido.EmSeparacao] = [StatusPedido.EmRota, StatusPedido.Cancelado],
            [StatusPedido.EmRota] = [StatusPedido.Entregue],
            [StatusPedido.Entregue] = [],
            [StatusPedido.Cancelado] = [],
        };

    private readonly List<ItemPedido> _itens = [];

    public int CompradorId { get; set; }

    public Usuario Comprador { get; set; } = null!;

    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    public StatusPedido Status { get; private set; } = StatusPedido.Rascunho;

    public decimal ValorTotal { get; private set; }

    public string EnderecoEntrega { get; set; } = string.Empty;

    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    public Entrega? Entrega { get; set; }

    public Fatura? Fatura { get; set; }

    public ICollection<MovimentoEstoque> Movimentos { get; set; } = new List<MovimentoEstoque>();

    /// <summary>Adiciona um item ao pedido e recalcula o total.</summary>
    public ItemPedido AdicionarItem(
        Produto produto,
        LoteColheita? lote,
        decimal quantidade,
        decimal precoUnitario)
    {
        ArgumentNullException.ThrowIfNull(produto);

        if (quantidade <= 0)
        {
            throw new RegraNegocioException("A quantidade do item deve ser maior que zero.");
        }

        if (precoUnitario < 0)
        {
            throw new RegraNegocioException("O preço unitário não pode ser negativo.");
        }

        if (Status is not (StatusPedido.Rascunho or StatusPedido.AguardandoAprovacao))
        {
            throw new RegraNegocioException(
                $"Não é possível alterar itens de um pedido com status {Status}.");
        }

        if (lote is not null && produto.Id != 0 && lote.ProdutoId != 0 && lote.ProdutoId != produto.Id)
        {
            throw new RegraNegocioException("O lote informado pertence a outro produto.");
        }

        var item = new ItemPedido
        {
            Pedido = this,
            PedidoId = Id,
            Produto = produto,
            ProdutoId = produto.Id,
            Lote = lote,
            LoteId = lote?.Id,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario,
            Subtotal = Arredondar(quantidade * precoUnitario),
        };

        _itens.Add(item);
        CalcularTotal();

        return item;
    }

    /// <summary>Remove um item do agregado e recalcula o total.</summary>
    public bool RemoverItem(ItemPedido item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!ReferenceEquals(item.Pedido, this) || !_itens.Remove(item))
        {
            return false;
        }

        CalcularTotal();

        return true;
    }

    /// <summary>Recalcula subtotais e o valor total do pedido.</summary>
    public decimal CalcularTotal()
    {
        foreach (var item in _itens)
        {
            item.Subtotal = Arredondar(item.Quantidade * item.PrecoUnitario);
        }

        ValorTotal = _itens.Sum(item => item.Subtotal);

        return ValorTotal;
    }

    /// <summary>Altera o status respeitando as transições permitidas.</summary>
    public void AlterarStatus(StatusPedido novoStatus)
    {
        if (novoStatus == Status)
        {
            return;
        }

        var permitidas = TransicoesPermitidas[Status];

        if (!permitidas.Contains(novoStatus))
        {
            throw new RegraNegocioException(
                $"Transição de {Status} para {novoStatus} não permitida.");
        }

        Status = novoStatus;
    }

    public bool PermiteTransicaoPara(StatusPedido novoStatus) =>
        novoStatus == Status || TransicoesPermitidas[Status].Contains(novoStatus);

    private static decimal Arredondar(decimal valor) =>
        decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
}
