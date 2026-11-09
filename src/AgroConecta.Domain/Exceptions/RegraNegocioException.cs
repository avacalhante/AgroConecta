namespace AgroConecta.Domain.Exceptions;

/// <summary>Regra de negócio violada. A camada Web traduz para HTTP 422.</summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}
