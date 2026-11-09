using AgroConecta.Application.Common;
using FluentValidation;

namespace AgroConecta.Application.Common;

public static class ValidacaoExtensions
{
    /// <summary>Executa o validador e converte as falhas em <see cref="ValidacaoException"/>.</summary>
    public static async Task ValidarAsync<T>(
        this IValidator<T> validador,
        T entrada,
        CancellationToken cancellationToken = default)
    {
        var resultado = await validador.ValidateAsync(entrada, cancellationToken);

        if (resultado.IsValid)
        {
            return;
        }

        var erros = resultado.Errors
            .GroupBy(falha => falha.PropertyName)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(falha => falha.ErrorMessage).ToArray());

        throw new ValidacaoException(erros);
    }
}
