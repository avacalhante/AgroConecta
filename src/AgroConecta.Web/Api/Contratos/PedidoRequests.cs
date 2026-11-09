using AgroConecta.Domain.Enums;

namespace AgroConecta.Web.Api.Contratos;

public record ItemPedidoRequest(
    int ProdutoId,
    int? LoteId,
    decimal Quantidade,
    decimal PrecoUnitario);

public record PedidoRequest(
    string EnderecoEntrega,
    IReadOnlyCollection<ItemPedidoRequest> Itens);

public record PedidoStatusRequest(StatusPedido Status, string? Observacao = null);
