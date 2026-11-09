using AgroConecta.Application.Relatorios;
using AgroConecta.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class RelatoriosController(IRelatoriosRepositorio relatorios) : Controller
{
    public async Task<IActionResult> VendasPorProdutor(
        DateOnly? inicio,
        DateOnly? fim,
        CancellationToken cancellationToken)
    {
        if (!User.IsInRole(nameof(PapelUsuario.Admin)))
        {
            return Forbid();
        }

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var periodoInicio = inicio ?? new DateOnly(hoje.Year, hoje.Month, 1);
        var periodoFim = fim ?? hoje;

        if (periodoFim < periodoInicio)
        {
            (periodoInicio, periodoFim) = (periodoFim, periodoInicio);
        }

        var vendas = await relatorios.VendasPorProdutorAsync(
            periodoInicio.ToDateTime(TimeOnly.MinValue),
            periodoFim.ToDateTime(TimeOnly.MaxValue),
            cancellationToken);

        ViewData["Inicio"] = periodoInicio;
        ViewData["Fim"] = periodoFim;

        return View(vendas);
    }
}
