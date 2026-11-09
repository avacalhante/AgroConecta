using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class EntregaRepositorio(AgroConectaDbContext contexto) : IEntregaRepositorio
{
    public Task<Entrega?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Entregas
            .Include(entrega => entrega.Pedido)
            .Include(entrega => entrega.Rota)
            .FirstOrDefaultAsync(entrega => entrega.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Entrega>> ListarAsync(
        int? motoristaId,
        StatusEntrega? status = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Entregas
            .AsNoTracking()
            .Include(entrega => entrega.Pedido)
            .Include(entrega => entrega.Rota)
            .AsQueryable();

        if (motoristaId is not null)
        {
            consulta = consulta.Where(entrega =>
                entrega.Rota != null && entrega.Rota.MotoristaId == motoristaId);
        }

        if (status is not null)
        {
            consulta = consulta.Where(entrega => entrega.Status == status);
        }

        return await consulta
            .OrderBy(entrega => entrega.DataPrevista)
            .ThenBy(entrega => entrega.Id)
            .ToListAsync(cancellationToken);
    }
}
