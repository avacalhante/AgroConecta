using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência dos produtores rurais.</summary>
public interface IProdutorRepositorio
{
    Task<Produtor?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task AdicionarAsync(Produtor produtor, CancellationToken cancellationToken = default);
}
