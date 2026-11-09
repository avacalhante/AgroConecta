using AgroConecta.Application.Consultas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class FaturasController(IConsultasWebRepositorio consultas) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var faturas = await consultas.ListarFaturasAsync(cancellationToken);
        return View(faturas);
    }
}
