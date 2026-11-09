namespace AgroConecta.Domain.Exceptions;

/// <summary>Conflito de unicidade em recurso existente (ex.: código de rastreio). A Web traduz para HTTP 409.</summary>
public class ConflitoException : Exception
{
    public ConflitoException(string mensagem) : base(mensagem)
    {
    }
}
