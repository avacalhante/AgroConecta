using AgroConecta.Application.Consultas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class EntregasWebController(IConsultasWebRepositorio consultas) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var entregas = await consultas.ListarEntregasAsync(cancellationToken);
        return View(entregas);
    }
}
