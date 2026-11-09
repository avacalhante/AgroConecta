using AgroConecta.Application.Common;
using AgroConecta.Application.Produtos;
using AgroConecta.Domain.Common;
using AgroConecta.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class ProdutosWebController(
    ICasoDeUso<ListarProdutosConsulta, ResultadoPaginado<ProdutoDto>> listarProdutos,
    ICasoDeUso<CriarProdutoComando, ProdutoDto> criarProduto) : Controller
{
    public async Task<IActionResult> Index(
        int page = 1,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var pagina = await listarProdutos.ExecutarAsync(
            new ListarProdutosConsulta(page, 20, ativo),
            cancellationToken);

        ViewData["Ativo"] = ativo;
        return View(pagina);
    }

    [HttpGet]
    [Authorize(Policy = "GerenciarCatalogo")]
    public IActionResult Criar() => View(new ProdutoViewModel());

    [HttpPost]
    [Authorize(Policy = "GerenciarCatalogo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        ProdutoViewModel modelo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await criarProduto.ExecutarAsync(
                new CriarProdutoComando(
                    modelo.Nome,
                    modelo.Categoria,
                    modelo.Unidade,
                    modelo.PrecoBase),
                cancellationToken);

            TempData["Mensagem"] = "Produto cadastrado com sucesso.";
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
