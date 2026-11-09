using AgroConecta.Application.Consultas;
using AgroConecta.Application.Indicadores;
using AgroConecta.Application.Pedidos;

namespace AgroConecta.Web.Models;

public record DashboardViewModel(
    IndicadoresDto Indicadores,
    IReadOnlyList<PedidoDto> PedidosRecentes,
    IReadOnlyList<EntregaResumo> EntregasRecentes,
    int FaturasPendentes,
    int ProdutoresAguardando);
