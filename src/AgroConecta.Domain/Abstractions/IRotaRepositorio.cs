using AgroConecta.Domain.Entities;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência das rotas de entrega.</summary>
public interface IRotaRepositorio
{
    Task<Rota?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Rota>> ListarAsync(CancellationToken cancellationToken = default);

    Task AdicionarAsync(Rota rota, CancellationToken cancellationToken = default);

    Task<bool> MotoristaAtivoExisteAsync(int motoristaId, CancellationToken cancellationToken = default);

    Task<bool> VeiculoAtivoExisteAsync(int veiculoId, CancellationToken cancellationToken = default);

    /// <summary>Indica se o pedido já está vinculado a alguma entrega.</summary>
    Task<bool> PedidoPossuiEntregaAsync(int pedidoId, CancellationToken cancellationToken = default);
}
