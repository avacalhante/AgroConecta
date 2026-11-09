namespace AgroConecta.Domain.Validacao;

/// <summary>Validação de CPF com dígitos verificadores.</summary>
public class ValidadorCpf : IValidadorDocumento
{
    public string Tipo => "CPF";

    public bool Validar(string documento)
    {
        var digitos = SomenteDigitos(documento);

        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
        {
            return false;
        }

        var primeiro = CalcularDigito(digitos, 9);
        var segundo = CalcularDigito(digitos, 10);

        return digitos[9] == primeiro && digitos[10] == segundo;
    }

    internal static string SomenteDigitos(string documento) =>
        new(documento.Where(char.IsDigit).ToArray());

    private static char CalcularDigito(string digitos, int posicao)
    {
        var soma = 0;

        for (var indice = 0; indice < posicao; indice++)
        {
            soma += (digitos[indice] - '0') * (posicao + 1 - indice);
        }

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;

        return (char)('0' + digito);
    }
}
