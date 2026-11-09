using AgroConecta.Application.Common;
using AgroConecta.Application.Lotes;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Web.Api.Contratos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/lotes")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class LotesController(
    ICasoDeUso<ListarLotesConsulta, ResultadoPaginado<LoteDto>> listar,
    ICasoDeUso<ListarMeusLotesConsulta, ResultadoPaginado<LoteDto>> listarMeus,
    ICasoDeUso<int, LoteDto> obter,
    ICasoDeUso<CriarLoteComando, LoteDto> criar) : ControllerBase
{
    /// <summary>Lista lotes de colheita com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResponse<LoteDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] StatusLote? status = null,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listar.ExecutarAsync(
            new ListarLotesConsulta(page, pageSize, status),
            cancellationToken);

        return Ok(PaginaResponse<LoteDto>.De(resultado));
    }

    /// <summary>Lista apenas os lotes do produtor autenticado.</summary>
    [HttpGet("meus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginaResponse<LoteDto>>> Meus(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listarMeus.ExecutarAsync(
            new ListarMeusLotesConsulta(User.ObterUsuarioId(), page, pageSize),
            cancellationToken);

        return Ok(PaginaResponse<LoteDto>.De(resultado));
    }

    /// <summary>Busca um lote pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoteDto>> Obter(int id, CancellationToken cancellationToken)
    {
        return Ok(await obter.ExecutarAsync(id, cancellationToken));
    }

    /// <summary>
    /// Registra um lote de colheita. O código de rastreio é gerado no cliente
    /// e a duplicidade retorna 409, permitindo sincronização idempotente.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "GerenciarCatalogo")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoteDto>> Criar(
        [FromBody] LoteRequest request,
        CancellationToken cancellationToken)
    {
        var comando = new CriarLoteComando(
            User.ObterUsuarioId(),
            request.ProdutoId,
            request.CodigoRastreio,
            request.DataColheita,
            request.Quantidade);

        var lote = await criar.ExecutarAsync(comando, cancellationToken);

        return CreatedAtAction(nameof(Obter), new { id = lote.Id }, lote);
    }
}
