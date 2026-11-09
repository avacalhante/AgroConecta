using AgroConecta.Application.Relatorios;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class RelatoriosRepositorio(AgroConectaDbContext contexto) : IRelatoriosRepositorio
{
    public async Task<IReadOnlyList<VendaProdutorResumo>> VendasPorProdutorAsync(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        var itens = await contexto.ItensPedido
            .AsNoTracking()
            .Where(item =>
                item.Lote != null
                && item.Pedido.DataPedido >= inicio
                && item.Pedido.DataPedido <= fim
                && item.Pedido.Status != StatusPedido.Cancelado)
            .Select(item => new
            {
                PedidoId = item.PedidoId,
                item.Subtotal,
                item.Quantidade,
                ProdutorId = item.Lote!.ProdutorId,
                Produtor = item.Lote.Produtor.Usuario.Nome,
                Cooperativa = item.Lote.Produtor.Cooperativa != null
                    ? item.Lote.Produtor.Cooperativa.Nome
                    : null,
                item.Lote.Produtor.Comunidade,
            })
            .ToListAsync(cancellationToken);

        return itens
            .GroupBy(item => new
            {
                item.ProdutorId,
                item.Produtor,
                item.Cooperativa,
                item.Comunidade,
            })
            .Select(grupo => new VendaProdutorResumo(
                grupo.Key.ProdutorId,
                grupo.Key.Produtor,
                grupo.Key.Cooperativa,
                grupo.Key.Comunidade,
                grupo.Select(item => item.PedidoId).Distinct().Count(),
                grupo.Sum(item => item.Quantidade),
                grupo.Sum(item => item.Subtotal)))
            .OrderByDescending(venda => venda.ValorTotal)
            .ToArray();
    }
}
