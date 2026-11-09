using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência dos lotes de colheita e sua rastreabilidade.</summary>
public interface ILoteRepositorio
{
    Task<LoteColheita?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<LoteColheita?> ObterPorCodigoRastreioAsync(
        Guid codigoRastreio,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteCodigoRastreioAsync(
        Guid codigoRastreio,
        CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<LoteColheita>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        StatusLote? status = null,
        int? produtorId = null,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(LoteColheita lote, CancellationToken cancellationToken = default);
}
