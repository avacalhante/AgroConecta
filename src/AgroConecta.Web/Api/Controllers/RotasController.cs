using AgroConecta.Application.Common;
using AgroConecta.Application.Rotas;
using AgroConecta.Web.Api.Contratos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/rotas")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SomenteAdmin")]
[Produces("application/json")]
public class RotasController(
    ICasoDeUso<bool, IReadOnlyCollection<RotaDto>> listar,
    ICasoDeUso<CriarRotaComando, RotaDto> criar) : ControllerBase
{
    /// <summary>Lista as rotas programadas.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RotaDto>>> Listar(
        CancellationToken cancellationToken)
    {
        return Ok(await listar.ExecutarAsync(true, cancellationToken));
    }

    /// <summary>Programa uma rota e cria as entregas dos pedidos selecionados.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RotaDto>> Criar(
        [FromBody] RotaRequest request,
        CancellationToken cancellationToken)
    {
        var rota = await criar.ExecutarAsync(
            new CriarRotaComando(
                request.Data,
                request.MotoristaId,
                request.VeiculoId,
                request.PedidoIds),
            cancellationToken);

        return CreatedAtAction(nameof(Listar), new { id = rota.Id }, rota);
    }
}
