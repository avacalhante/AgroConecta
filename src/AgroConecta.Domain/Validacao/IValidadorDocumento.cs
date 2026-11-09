namespace AgroConecta.Domain.Validacao;

/// <summary>
/// Contrato polimórfico de validação de documentos (CPF/CNPJ).
/// Permite trocar a implementação sem alterar os casos de uso (LSP + DIP).
/// </summary>
public interface IValidadorDocumento
{
    string Tipo { get; }

    bool Validar(string documento);
}
