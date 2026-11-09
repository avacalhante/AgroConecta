using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class FaturaRepositorio(AgroConectaDbContext contexto) : IFaturaRepositorio
{
    public Task<Fatura?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Faturas
            .Include(fatura => fatura.Pedido)
            .FirstOrDefaultAsync(fatura => fatura.Id == id, cancellationToken);

    public Task<Fatura?> ObterPorPedidoAsync(int pedidoId, CancellationToken cancellationToken = default) =>
        contexto.Faturas
            .Include(fatura => fatura.Pedido)
            .FirstOrDefaultAsync(fatura => fatura.PedidoId == pedidoId, cancellationToken);

    public async Task<IReadOnlyCollection<Fatura>> ListarAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Faturas
            .AsNoTracking()
            .Include(fatura => fatura.Pedido)
            .OrderByDescending(fatura => fatura.Id)
            .ToListAsync(cancellationToken);

    public async Task AdicionarAsync(Fatura fatura, CancellationToken cancellationToken = default) =>
        await contexto.Faturas.AddAsync(fatura, cancellationToken);
}
