using AgroConecta.Application.Relatorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/relatorios")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SomenteAdmin")]
[Produces("application/json")]
public class RelatoriosController(IRelatoriosRepositorio relatorios) : ControllerBase
{
    /// <summary>Relatório de vendas por produtor no período informado.</summary>
    [HttpGet("vendas-produtor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<VendaProdutorResumo>>> VendasPorProdutor(
        [FromQuery] DateOnly? inicio = null,
        [FromQuery] DateOnly? fim = null,
        CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var primeiroDiaDoMes = new DateOnly(hoje.Year, hoje.Month, 1);

        var periodoInicio = inicio ?? primeiroDiaDoMes;
        var periodoFim = fim ?? hoje;

        if (periodoFim < periodoInicio)
        {
            throw new AgroConecta.Application.Common.ValidacaoException(
                new Dictionary<string, string[]>
                {
                    [nameof(fim)] = ["A data final deve ser posterior à data inicial."],
                });
        }

        var resultado = await relatorios.VendasPorProdutorAsync(
            periodoInicio.ToDateTime(TimeOnly.MinValue),
            periodoFim.ToDateTime(TimeOnly.MaxValue),
            cancellationToken);

        return Ok(resultado);
    }
}
