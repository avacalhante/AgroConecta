namespace AgroConecta.Domain.Exceptions;

/// <summary>Recurso inexistente. A camada Web traduz para HTTP 404.</summary>
public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}
