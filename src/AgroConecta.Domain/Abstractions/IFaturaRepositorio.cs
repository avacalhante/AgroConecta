using AgroConecta.Domain.Entities;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência das faturas.</summary>
public interface IFaturaRepositorio
{
    Task<Fatura?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Fatura?> ObterPorPedidoAsync(int pedidoId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Fatura>> ListarAsync(CancellationToken cancellationToken = default);

    Task AdicionarAsync(Fatura fatura, CancellationToken cancellationToken = default);
}
