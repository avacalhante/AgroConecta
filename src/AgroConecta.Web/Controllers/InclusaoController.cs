using AgroConecta.Application.Common;
using AgroConecta.Application.Consultas;
using AgroConecta.Application.Indicadores;
using AgroConecta.Domain.Enums;
using AgroConecta.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroConecta.Web.Controllers;

[Authorize]
public class InclusaoController(
    ICasoDeUso<bool, IndicadoresDto> obterIndicadores,
    IConsultasWebRepositorio consultas) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var indicadores = await obterIndicadores.ExecutarAsync(true, cancellationToken);
        var produtores = await consultas.ListarProdutoresAsync(null, cancellationToken);

        var porComunidade = produtores
            .GroupBy(produtor => produtor.Comunidade.ToString())
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());

        return View(new InclusaoViewModel(
            indicadores,
            produtores,
            porComunidade,
            produtores.Count,
            produtores.Count(produtor => produtor.StatusCadastro == StatusCadastro.Aprovado),
            produtores.Count(produtor => produtor.StatusCadastro == StatusCadastro.Pendente)));
    }
}
