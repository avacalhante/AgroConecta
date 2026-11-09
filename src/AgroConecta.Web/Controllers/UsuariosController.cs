using AgroConecta.Application.Consultas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize(Policy = "SomenteAdmin")]
public class UsuariosController(IConsultasWebRepositorio consultas) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var usuarios = await consultas.ListarUsuariosAsync(cancellationToken);
        return View(usuarios);
    }

    public async Task<IActionResult> Auditoria(CancellationToken cancellationToken)
    {
        var logs = await consultas.ListarLogsAsync(200, cancellationToken);
        return View(logs);
    }
}
