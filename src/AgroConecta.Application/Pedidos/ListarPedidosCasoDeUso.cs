using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;

namespace AgroConecta.Application.Pedidos;

public class ListarPedidosCasoDeUso(IPedidoRepositorio repositorio)
    : ICasoDeUso<ListarPedidosConsulta, ResultadoPaginado<PedidoDto>>
{
    public async Task<ResultadoPaginado<PedidoDto>> ExecutarAsync(
        ListarPedidosConsulta entrada,
        CancellationToken cancellationToken = default)
    {
        var pagina = Math.Max(entrada.Pagina, 1);
        var tamanhoPagina = Math.Clamp(entrada.TamanhoPagina, 1, 100);

        var resultado = await repositorio.ListarAsync(
            pagina,
            tamanhoPagina,
            entrada.Status,
            entrada.CompradorId,
            cancellationToken);

        return new ResultadoPaginado<PedidoDto>(
            resultado.Itens.Select(MapeadorPedido.ParaDto).ToArray(),
            resultado.Total,
            resultado.Pagina,
            resultado.TamanhoPagina);
    }
}

public static class MapeadorPedido
{
    public static PedidoDto ParaDto(Pedido pedido) => new(
        pedido.Id,
        pedido.CompradorId,
        pedido.Comprador?.Nome ?? string.Empty,
        pedido.DataPedido,
        pedido.Status,
        pedido.ValorTotal,
        pedido.EnderecoEntrega,
        pedido.Itens
            .Select(item => new ItemPedidoDto(
                item.Id,
                item.ProdutoId,
                item.Produto?.Nome ?? string.Empty,
                item.LoteId,
                item.Quantidade,
                item.PrecoUnitario,
                item.Subtotal))
            .ToArray());
}
