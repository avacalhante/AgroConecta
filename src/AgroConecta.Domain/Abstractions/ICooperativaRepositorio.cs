using AgroConecta.Domain.Entities;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência das cooperativas.</summary>
public interface ICooperativaRepositorio
{
    Task<Cooperativa?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task AdicionarAsync(Cooperativa cooperativa, CancellationToken cancellationToken = default);
}
