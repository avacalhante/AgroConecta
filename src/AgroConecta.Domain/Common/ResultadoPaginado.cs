namespace AgroConecta.Domain.Common;

/// <summary>Página de resultados usada pelas listagens da API.</summary>
public class ResultadoPaginado<T>
{
    public ResultadoPaginado(IReadOnlyCollection<T> itens, int total, int pagina, int tamanhoPagina)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        ArgumentOutOfRangeException.ThrowIfLessThan(pagina, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanhoPagina, 1);

        Itens = itens;
        Total = total;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
    }

    public IReadOnlyCollection<T> Itens { get; }

    public int Total { get; }

    public int Pagina { get; }

    public int TamanhoPagina { get; }
}
