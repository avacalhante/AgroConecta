using System.Diagnostics;
using AgroConecta.Application.Common;
using AgroConecta.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api;

/// <summary>
/// Converte exceções do domínio e da aplicação no padrão RFC 7807,
/// conforme contracts/errors.md.
/// </summary>
public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    private const string BaseErros = "https://agroconecta.local/errors";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, tipo, titulo) = exception switch
        {
            ValidacaoException => (StatusCodes.Status400BadRequest, "validacao", "Erro de validação"),
            RegraNegocioException => (StatusCodes.Status422UnprocessableEntity, "regra-negocio", "Regra de negócio violada"),
            RecursoNaoEncontradoException => (StatusCodes.Status404NotFound, "nao-encontrado", "Recurso não encontrado"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "nao-autenticado", "Não autenticado"),
            _ => (StatusCodes.Status500InternalServerError, "inesperado", "Erro inesperado"),
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Erro não tratado em {Caminho}", httpContext.Request.Path);
        }
        else
        {
            logger.LogInformation(
                "Requisição rejeitada em {Caminho}: {Mensagem}",
                httpContext.Request.Path,
                exception.Message);
        }

        var problema = new ProblemDetails
        {
            Status = status,
            Type = $"{BaseErros}/{tipo}",
            Title = titulo,
            Detail = status >= StatusCodes.Status500InternalServerError
                ? "Não foi possível concluir a operação."
                : exception.Message,
            Instance = httpContext.Request.Path,
        };

        problema.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (exception is ValidacaoException validacao)
        {
            problema.Extensions["errors"] = validacao.Erros;
        }

        httpContext.Response.StatusCode = status;
        await Results.Problem(problema).ExecuteAsync(httpContext);

        return true;
    }
}
