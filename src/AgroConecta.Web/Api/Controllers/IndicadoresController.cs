using AgroConecta.Application.Common;
using AgroConecta.Application.Indicadores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/indicadores")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class IndicadoresController(
    ICasoDeUso<bool, IndicadoresDto> obterIndicadores) : ControllerBase
{
    /// <summary>Retorna os indicadores do dashboard.</summary>
    [HttpGet]
    [Authorize(Policy = "SomenteAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IndicadoresDto>> Obter(CancellationToken cancellationToken)
    {
        return Ok(await obterIndicadores.ExecutarAsync(true, cancellationToken));
    }
}
