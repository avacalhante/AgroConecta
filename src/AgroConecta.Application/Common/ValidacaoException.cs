namespace AgroConecta.Application.Common;

/// <summary>Entrada inválida detectada pelos validadores. A Web traduz para HTTP 400.</summary>
public class ValidacaoException : Exception
{
    public ValidacaoException(IDictionary<string, string[]> erros)
        : base("Um ou mais campos são inválidos.")
    {
        Erros = erros;
    }

    public IDictionary<string, string[]> Erros { get; }
}
