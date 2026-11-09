using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Relatorios;

/// <summary>Consolidado de vendas por produtor em um período.</summary>
public record VendaProdutorResumo(
    int ProdutorId,
    string Produtor,
    string? Cooperativa,
    Comunidade Comunidade,
    int Pedidos,
    decimal Quantidade,
    decimal ValorTotal);

public record PeriodoConsulta(DateOnly Inicio, DateOnly Fim);

public interface IRelatoriosRepositorio
{
    Task<IReadOnlyList<VendaProdutorResumo>> VendasPorProdutorAsync(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default);
}
