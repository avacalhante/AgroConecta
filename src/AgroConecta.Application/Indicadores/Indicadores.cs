using AgroConecta.Application.Common;

namespace AgroConecta.Application.Indicadores;

public record IndicadoresDto(
    int PedidosMes,
    double EntregasNoPrazoPercentual,
    double PerdasPercentual,
    int ProdutoresAtivos,
    int CooperativasAtivas,
    double EntregasComEvidenciaPercentual,
    double TelasAcessiveisPercentual);

/// <summary>Consulta agregada dos indicadores do dashboard.</summary>
public interface IIndicadoresRepositorio
{
    Task<IndicadoresDto> ObterAsync(
        DateTime referencia,
        CancellationToken cancellationToken = default);
}

public class ObterIndicadoresCasoDeUso(IIndicadoresRepositorio repositorio)
    : ICasoDeUso<bool, IndicadoresDto>
{
    /// <summary>O parâmetro de entrada não é usado; mantido para o contrato genérico de caso de uso.</summary>
    public Task<IndicadoresDto> ExecutarAsync(
        bool entrada,
        CancellationToken cancellationToken = default) =>
        repositorio.ObterAsync(DateTime.UtcNow, cancellationToken);
}
