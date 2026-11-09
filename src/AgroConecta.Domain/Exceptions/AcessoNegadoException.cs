namespace AgroConecta.Domain.Exceptions;

/// <summary>Usuário autenticado sem permissão para a operação. A Web traduz para HTTP 403.</summary>
public class AcessoNegadoException : Exception
{
    public AcessoNegadoException(string mensagem) : base(mensagem)
    {
    }
}
