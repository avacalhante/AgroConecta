using System.Diagnostics;
using AgroConecta.Application.Common;
using AgroConecta.Application.Consultas;
using AgroConecta.Application.Indicadores;
using AgroConecta.Application.Pedidos;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Web.Api;
using AgroConecta.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class HomeController(
    ICasoDeUso<bool, IndicadoresDto> obterIndicadores,
    ICasoDeUso<ListarPedidosConsulta, ResultadoPaginado<PedidoDto>> listarPedidos,
    IConsultasWebRepositorio consultas) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var compradorId = User.EhAdmin() ? (int?)null : User.ObterUsuarioId();

        var indicadores = await obterIndicadores.ExecutarAsync(true, cancellationToken);
        var pedidos = await listarPedidos.ExecutarAsync(
            new ListarPedidosConsulta(1, 5, null, compradorId),
            cancellationToken);
        var entregas = await consultas.ListarEntregasAsync(cancellationToken);
        var faturas = await consultas.ListarFaturasAsync(cancellationToken);
        var produtores = await consultas.ListarProdutoresAsync(StatusCadastro.Pendente, cancellationToken);

        return View(new DashboardViewModel(
            indicadores,
            pedidos.Itens.ToArray(),
            entregas.Take(5).ToArray(),
            faturas.Count(fatura => fatura.Status != StatusFatura.Paga),
            produtores.Count));
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    public IActionResult Privacidade() => View();
}
