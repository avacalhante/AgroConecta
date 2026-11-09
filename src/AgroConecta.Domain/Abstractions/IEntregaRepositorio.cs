using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência das entregas e sua relação com rotas e pedidos.</summary>
public interface IEntregaRepositorio
{
    Task<Entrega?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Entrega>> ListarAsync(
        int? motoristaId,
        StatusEntrega? status = null,
        CancellationToken cancellationToken = default);
}
