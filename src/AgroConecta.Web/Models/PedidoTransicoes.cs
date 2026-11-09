using AgroConecta.Domain.Enums;

namespace AgroConecta.Web.Models;

/// <summary>Transições de status exibidas na tela de detalhe do pedido.</summary>
public static class PedidoTransicoes
{
    private static readonly IReadOnlyDictionary<StatusPedido, StatusPedido[]> Permitidas =
        new Dictionary<StatusPedido, StatusPedido[]>
        {
            [StatusPedido.Rascunho] = [StatusPedido.AguardandoAprovacao, StatusPedido.Cancelado],
            [StatusPedido.AguardandoAprovacao] = [StatusPedido.Aprovado, StatusPedido.Cancelado],
            [StatusPedido.Aprovado] = [StatusPedido.EmSeparacao, StatusPedido.Cancelado],
            [StatusPedido.EmSeparacao] = [StatusPedido.EmRota, StatusPedido.Cancelado],
            [StatusPedido.EmRota] = [StatusPedido.Entregue],
            [StatusPedido.Entregue] = [],
            [StatusPedido.Cancelado] = [],
        };

    public static IReadOnlyList<StatusPedido> Proximos(StatusPedido atual) =>
        Permitidas.TryGetValue(atual, out var proximos) ? proximos : [];
}
