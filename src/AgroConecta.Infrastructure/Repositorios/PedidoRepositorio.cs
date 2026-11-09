using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class PedidoRepositorio(AgroConectaDbContext contexto) : IPedidoRepositorio
{
    public Task<Pedido?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Pedidos
            .Include(pedido => pedido.Comprador)
            .Include(pedido => pedido.Itens)
            .ThenInclude(item => item.Produto)
            .Include(pedido => pedido.Entrega)
            .Include(pedido => pedido.Fatura)
            .AsSplitQuery()
            .FirstOrDefaultAsync(pedido => pedido.Id == id, cancellationToken);

    public async Task<ResultadoPaginado<Pedido>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        StatusPedido? status = null,
        int? compradorId = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Pedidos
            .AsNoTracking()
            .Include(pedido => pedido.Comprador)
            .Include(pedido => pedido.Itens)
            .ThenInclude(item => item.Produto)
            .AsSplitQuery();

        if (status is not null)
        {
            consulta = consulta.Where(pedido => pedido.Status == status);
        }

        if (compradorId is not null)
        {
            consulta = consulta.Where(pedido => pedido.CompradorId == compradorId);
        }

        var total = await consulta.CountAsync(cancellationToken);
        var itens = await consulta
            .OrderByDescending(pedido => pedido.DataPedido)
            .ThenByDescending(pedido => pedido.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Pedido>(itens, total, pagina, tamanhoPagina);
    }

    public async Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default) =>
        await contexto.Pedidos.AddAsync(pedido, cancellationToken);
}
