using AgroConecta.Application.Common;
using AgroConecta.Application.Pedidos;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using AgroConecta.Web.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class PedidosWebController(
    ICasoDeUso<ListarPedidosConsulta, ResultadoPaginado<PedidoDto>> listarPedidos,
    ICasoDeUso<int, PedidoDto> obterPedido,
    ICasoDeUso<AlterarStatusPedidoComando, PedidoDto> alterarStatus) : Controller
{
    public async Task<IActionResult> Index(
        int page = 1,
        StatusPedido? status = null,
        CancellationToken cancellationToken = default)
    {
        var compradorId = User.EhAdmin() ? (int?)null : User.ObterUsuarioId();

        var pagina = await listarPedidos.ExecutarAsync(
            new ListarPedidosConsulta(page, 20, status, compradorId),
            cancellationToken);

        ViewData["Status"] = status;
        return View(pagina);
    }

    public async Task<IActionResult> Detalhe(int id, CancellationToken cancellationToken)
    {
        try
        {
            var pedido = await obterPedido.ExecutarAsync(id, cancellationToken);

            if (!User.EhAdmin() && pedido.CompradorId != User.ObterUsuarioId())
            {
                return NotFound();
            }

            return View(pedido);
        }
        catch (RecursoNaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Policy = "OperarPedidos")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(
        int id,
        StatusPedido status,
        CancellationToken cancellationToken)
    {
        try
        {
            await alterarStatus.ExecutarAsync(
                new AlterarStatusPedidoComando(id, status),
                cancellationToken);

            TempData["Mensagem"] = $"Status do pedido atualizado para {status}.";
        }
        catch (RegraNegocioException excecao)
        {
            TempData["Erro"] = excecao.Message;
        }
        catch (RecursoNaoEncontradoException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Detalhe), new { id });
    }
}
