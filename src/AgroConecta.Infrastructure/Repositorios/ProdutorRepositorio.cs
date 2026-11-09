using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class ProdutorRepositorio(AgroConectaDbContext contexto) : IProdutorRepositorio
{
    public Task<Produtor?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Produtores
            .Include(produtor => produtor.Usuario)
            .Include(produtor => produtor.Cooperativa)
            .FirstOrDefaultAsync(produtor => produtor.Id == id, cancellationToken);

    public async Task AdicionarAsync(Produtor produtor, CancellationToken cancellationToken = default) =>
        await contexto.Produtores.AddAsync(produtor, cancellationToken);
}
