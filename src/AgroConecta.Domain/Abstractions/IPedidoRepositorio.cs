using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência do agregado Pedido.</summary>
public interface IPedidoRepositorio
{
    Task<Pedido?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<Pedido>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        StatusPedido? status = null,
        int? compradorId = null,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default);
}
