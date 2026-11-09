using AgroConecta.Domain.Abstractions;
using AgroConecta.Infrastructure.Persistence;

namespace AgroConecta.Infrastructure.Repositorios;

public class UnidadeDeTrabalho(AgroConectaDbContext contexto) : IUnidadeDeTrabalho
{
    public Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        contexto.SaveChangesAsync(cancellationToken);
}
