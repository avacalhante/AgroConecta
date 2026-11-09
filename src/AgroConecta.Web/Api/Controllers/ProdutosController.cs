using AgroConecta.Application.Common;
using AgroConecta.Application.Produtos;
using AgroConecta.Web.Api.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/produtos")]
[Produces("application/json")]
public class ProdutosController(
    ICasoDeUso<ListarProdutosConsulta, Domain.Common.ResultadoPaginado<ProdutoDto>> listar,
    ICasoDeUso<int, ProdutoDto> obter,
    ICasoDeUso<CriarProdutoComando, ProdutoDto> criar) : ControllerBase
{
    /// <summary>Lista o catálogo de produtos com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResponse<ProdutoDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listar.ExecutarAsync(
            new ListarProdutosConsulta(page, pageSize, ativo),
            cancellationToken);

        return Ok(PaginaResponse<ProdutoDto>.De(resultado));
    }

    /// <summary>Busca um produto pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> Obter(int id, CancellationToken cancellationToken)
    {
        return Ok(await obter.ExecutarAsync(id, cancellationToken));
    }

    /// <summary>Cria um produto no catálogo.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDto>> Criar(
        [FromBody] CriarProdutoComando comando,
        CancellationToken cancellationToken)
    {
        var produto = await criar.ExecutarAsync(comando, cancellationToken);

        return CreatedAtAction(nameof(Obter), new { id = produto.Id }, produto);
    }
}
