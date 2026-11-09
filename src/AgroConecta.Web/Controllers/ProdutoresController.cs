using AgroConecta.Application.Common;
using AgroConecta.Application.Consultas;
using AgroConecta.Application.Produtores;
using AgroConecta.Domain.Enums;
using AgroConecta.Web.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class ProdutoresController(
    IConsultasWebRepositorio consultas,
    ICasoDeUso<AprovarProdutorComando, bool> aprovarProdutor) : Controller
{
    public async Task<IActionResult> Index(
        StatusCadastro? status,
        CancellationToken cancellationToken)
    {
        ViewData["Status"] = status;
        var produtores = await consultas.ListarProdutoresAsync(status, cancellationToken);

        return View(produtores);
    }

    [HttpPost]
    [Authorize(Policy = "SomenteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aprovar(int id, bool aprovar, CancellationToken cancellationToken)
    {
        await aprovarProdutor.ExecutarAsync(
            new AprovarProdutorComando(id, aprovar, User.ObterUsuarioId()),
            cancellationToken);

        TempData["Mensagem"] = aprovar
            ? "Cadastro do produtor aprovado."
            : "Cadastro do produtor rejeitado.";

        return RedirectToAction(nameof(Index));
    }
}
