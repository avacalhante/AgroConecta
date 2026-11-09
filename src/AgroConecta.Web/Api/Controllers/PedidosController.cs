using AgroConecta.Application.Common;
using AgroConecta.Application.Pedidos;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using AgroConecta.Web.Api.Contratos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Api.Controllers;

[ApiController]
[Route("api/v1/pedidos")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class PedidosController(
    ICasoDeUso<ListarPedidosConsulta, ResultadoPaginado<PedidoDto>> listar,
    ICasoDeUso<CriarPedidoComando, PedidoDto> criar,
    ICasoDeUso<int, PedidoDto> obter,
    ICasoDeUso<AlterarStatusPedidoComando, PedidoDto> alterarStatus) : ControllerBase
{
    /// <summary>Lista pedidos. Compradores veem apenas os próprios; Admin vê todos.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResponse<PedidoDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] StatusPedido? status = null,
        CancellationToken cancellationToken = default)
    {
        var compradorId = User.EhAdmin() ? (int?)null : User.ObterUsuarioId();

        var resultado = await listar.ExecutarAsync(
            new ListarPedidosConsulta(page, pageSize, status, compradorId),
            cancellationToken);

        return Ok(PaginaResponse<PedidoDto>.De(resultado));
    }

    /// <summary>Cria um pedido para o comprador autenticado.</summary>
    [HttpPost]
    [Authorize(Policy = "Comprar")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PedidoDto>> Criar(
        [FromBody] PedidoRequest request,
        CancellationToken cancellationToken)
    {
        var comando = new CriarPedidoComando(
            User.ObterUsuarioId(),
            request.EnderecoEntrega,
            request.Itens
                .Select(item => new CriarItemPedidoComando(
                    item.ProdutoId,
                    item.LoteId,
                    item.Quantidade,
                    item.PrecoUnitario))
                .ToArray());

        var pedido = await criar.ExecutarAsync(comando, cancellationToken);

        return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, pedido);
    }

    /// <summary>Busca um pedido. Compradores só acessam os próprios pedidos.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoDto>> Obter(int id, CancellationToken cancellationToken)
    {
        var pedido = await obter.ExecutarAsync(id, cancellationToken);

        if (!User.EhAdmin() && pedido.CompradorId != User.ObterUsuarioId())
        {
            // 404 para não revelar a existência de pedidos de outros compradores.
            throw new RecursoNaoEncontradoException("Pedido não encontrado.");
        }

        return Ok(pedido);
    }

    /// <summary>Altera o status do pedido respeitando as transições de negócio.</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "OperarPedidos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PedidoDto>> AlterarStatus(
        int id,
        [FromBody] PedidoStatusRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await alterarStatus.ExecutarAsync(
            new AlterarStatusPedidoComando(id, request.Status, request.Observacao),
            cancellationToken));
    }
}
