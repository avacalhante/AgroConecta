using AgroConecta.Application.Consultas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/auditoria")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SomenteAdmin")]
[Produces("application/json")]
public class AuditoriaController(IConsultasWebRepositorio consultas) : ControllerBase
{
    /// <summary>Lista os registros de auditoria mais recentes.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<LogAcessoResumo>>> Listar(
        [FromQuery] int limite = 100,
        CancellationToken cancellationToken = default)
    {
        return Ok(await consultas.ListarLogsAsync(limite, cancellationToken));
    }
}
