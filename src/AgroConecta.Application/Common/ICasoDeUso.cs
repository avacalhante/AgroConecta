namespace AgroConecta.Application.Common;

/// <summary>
/// Contrato base dos casos de uso. A Web depende desta abstração (DIP),
/// não das implementações concretas.
/// </summary>
public interface ICasoDeUso<in TEntrada, TSaida>
{
    Task<TSaida> ExecutarAsync(TEntrada entrada, CancellationToken cancellationToken = default);
}
