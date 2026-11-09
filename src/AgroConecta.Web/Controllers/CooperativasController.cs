using AgroConecta.Application.Common;
using AgroConecta.Application.Cooperativas;
using AgroConecta.Application.Consultas;
using AgroConecta.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class CooperativasController(
    IConsultasWebRepositorio consultas,
    ICasoDeUso<CriarCooperativaComando, int> criarCooperativa) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var cooperativas = await consultas.ListarCooperativasAsync(cancellationToken);
        return View(cooperativas);
    }

    [HttpGet]
    [Authorize(Policy = "SomenteAdmin")]
    public IActionResult Criar() => View(new CooperativaViewModel());

    [HttpPost]
    [Authorize(Policy = "SomenteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        CooperativaViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await criarCooperativa.ExecutarAsync(
                new CriarCooperativaComando(modelo.Nome, modelo.Cidade, modelo.UF, modelo.Tipo),
                cancellationToken);

            TempData["Mensagem"] = "Cooperativa cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidacaoException excecao)
        {
            foreach (var (campo, mensagens) in excecao.Erros)
            {
                foreach (var mensagem in mensagens)
                {
                    ModelState.AddModelError(campo, mensagem);
                }
            }
        }

        return View(modelo);
    }
}
