using AgroConecta.Application.Consultas;
using AgroConecta.Application.Indicadores;

namespace AgroConecta.Web.Models;

public record InclusaoViewModel(
    IndicadoresDto Indicadores,
    IReadOnlyList<ProdutorResumo> Produtores,
    IReadOnlyDictionary<string, int> PorComunidade,
    int TotalProdutores,
    int Aprovados,
    int Pendentes);
