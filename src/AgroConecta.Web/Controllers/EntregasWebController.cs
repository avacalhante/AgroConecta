using AgroConecta.Application.Common;
using AgroConecta.Application.Consultas;
using AgroConecta.Application.Rotas;
using AgroConecta.Domain.Enums;
using AgroConecta.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class EntregasWebController(
    IConsultasWebRepositorio consultas,
    ICasoDeUso<bool, IReadOnlyCollection<RotaDto>> listarRotas,
    ICasoDeUso<CriarRotaComando, RotaDto> criarRota) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var entregas = await consultas.ListarEntregasAsync(cancellationToken);
        var rotas = await listarRotas.ExecutarAsync(true, cancellationToken);

        return View(new EntregasViewModel(entregas, rotas));
    }

    [HttpGet]
    [Authorize(Policy = "SomenteAdmin")]
    public async Task<IActionResult> Criar(CancellationToken cancellationToken)
    {
        return View(await MontarModeloAsync(cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = "SomenteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        RotaViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await MontarModeloAsync(modelo, cancellationToken));
        }

        try
        {
            await criarRota.ExecutarAsync(
                new CriarRotaComando(
                    modelo.Data,
                    modelo.MotoristaId,
                    modelo.VeiculoId,
                    modelo.PedidoIds),
                cancellationToken);

            TempData["Mensagem"] = "Rota programada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Domain.Exceptions.RegraNegocioException excecao)
        {
            ModelState.AddModelError(string.Empty, excecao.Message);
        }
        catch (Domain.Exceptions.RecursoNaoEncontradoException excecao)
        {
            ModelState.AddModelError(string.Empty, excecao.Message);
        }

        return View(await MontarModeloAsync(modelo, cancellationToken));
    }

    private async Task<RotaViewModel> MontarModeloAsync(CancellationToken cancellationToken) =>
        await MontarModeloAsync(new RotaViewModel(), cancellationToken);

    private async Task<RotaViewModel> MontarModeloAsync(
        RotaViewModel modelo,
        CancellationToken cancellationToken)
    {
        modelo.Motoristas = await consultas.ListarMotoristasAsync(cancellationToken);
        modelo.Veiculos = await consultas.ListarVeiculosAsync(cancellationToken);
        modelo.Pedidos = await consultas.ListarPedidosSemEntregaAsync(cancellationToken);
        return modelo;
    }
}
