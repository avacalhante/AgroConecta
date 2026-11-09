namespace AgroConecta.Domain.Validacao;

/// <summary>Validação de CNPJ com dígitos verificadores.</summary>
public class ValidadorCnpj : IValidadorDocumento
{
    private static readonly int[] PesosPrimeiroDigito = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosSegundoDigito = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public string Tipo => "CNPJ";

    public bool Validar(string documento)
    {
        var digitos = ValidadorCpf.SomenteDigitos(documento);

        if (digitos.Length != 14 || digitos.Distinct().Count() == 1)
        {
            return false;
        }

        var primeiro = CalcularDigito(digitos, PesosPrimeiroDigito);
        var segundo = CalcularDigito(digitos, PesosSegundoDigito);

        return digitos[12] == primeiro && digitos[13] == segundo;
    }

    private static char CalcularDigito(string digitos, int[] pesos)
    {
        var soma = 0;

        for (var indice = 0; indice < pesos.Length; indice++)
        {
            soma += (digitos[indice] - '0') * pesos[indice];
        }

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;

        return (char)('0' + digito);
    }
}
