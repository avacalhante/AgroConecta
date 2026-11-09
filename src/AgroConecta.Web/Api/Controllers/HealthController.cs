using AgroConecta.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("health")]
[AllowAnonymous]
public class HealthController(AgroConectaDbContext contexto) : ControllerBase
{
    /// <summary>Verifica a saúde da aplicação e a conexão com o banco.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Obter(CancellationToken cancellationToken)
    {
        var versao = typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "0.1.0";
        var conectado = await contexto.Database.CanConnectAsync(cancellationToken);

        if (!conectado)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { status = "indisponivel", version = versao });
        }

        return Ok(new { status = "ok", version = versao });
    }
}
