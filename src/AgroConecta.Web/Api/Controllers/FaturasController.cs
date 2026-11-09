using AgroConecta.Application.Common;
using AgroConecta.Application.Faturas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/faturas")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SomenteAdmin")]
[Produces("application/json")]
public class FaturasController(
    ICasoDeUso<bool, IReadOnlyCollection<FaturaDto>> listar,
    ICasoDeUso<GerarFaturaComando, FaturaDto> gerar) : ControllerBase
{
    /// <summary>Lista as faturas emitidas.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<FaturaDto>>> Listar(
        CancellationToken cancellationToken)
    {
        return Ok(await listar.ExecutarAsync(true, cancellationToken));
    }

    /// <summary>Gera a fatura de um pedido. A operação é idempotente por pedido.</summary>
    [HttpPost("pedido/{pedidoId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<FaturaDto>> Gerar(
        int pedidoId,
        CancellationToken cancellationToken)
    {
        var fatura = await gerar.ExecutarAsync(
            new GerarFaturaComando(pedidoId),
            cancellationToken);

        return CreatedAtAction(nameof(Listar), new { id = fatura.Id }, fatura);
    }
}
