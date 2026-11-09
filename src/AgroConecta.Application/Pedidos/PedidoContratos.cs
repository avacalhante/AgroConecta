using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Pedidos;

public record ItemPedidoDto(
    int Id,
    int ProdutoId,
    string ProdutoNome,
    int? LoteId,
    decimal Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal);

public record PedidoDto(
    int Id,
    int CompradorId,
    string CompradorNome,
    DateTime DataPedido,
    StatusPedido Status,
    decimal ValorTotal,
    string EnderecoEntrega,
    IReadOnlyCollection<ItemPedidoDto> Itens);

public record CriarItemPedidoComando(
    int ProdutoId,
    int? LoteId,
    decimal Quantidade,
    decimal PrecoUnitario);

public record CriarPedidoComando(
    int CompradorId,
    string EnderecoEntrega,
    IReadOnlyCollection<CriarItemPedidoComando> Itens);

public record ListarPedidosConsulta(
    int Pagina,
    int TamanhoPagina,
    StatusPedido? Status = null,
    int? CompradorId = null);

public record AlterarStatusPedidoComando(int PedidoId, StatusPedido Status, string? Observacao = null);
