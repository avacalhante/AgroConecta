using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class CooperativaRepositorio(AgroConectaDbContext contexto) : ICooperativaRepositorio
{
    public Task<Cooperativa?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Cooperativas.FirstOrDefaultAsync(cooperativa => cooperativa.Id == id, cancellationToken);

    public async Task AdicionarAsync(Cooperativa cooperativa, CancellationToken cancellationToken = default) =>
        await contexto.Cooperativas.AddAsync(cooperativa, cancellationToken);
}
