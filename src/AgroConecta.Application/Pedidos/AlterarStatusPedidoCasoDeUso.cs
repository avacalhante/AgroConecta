using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Pedidos;

public class AlterarStatusPedidoCasoDeUso(
    IPedidoRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho)
    : ICasoDeUso<AlterarStatusPedidoComando, PedidoDto>
{
    public async Task<PedidoDto> ExecutarAsync(
        AlterarStatusPedidoComando entrada,
        CancellationToken cancellationToken = default)
    {
        var pedido = await repositorio.ObterPorIdAsync(entrada.PedidoId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Pedido não encontrado.");

        ValidarEntregaExigida(pedido.Entrega, entrada.Status);

        pedido.AlterarStatus(entrada.Status);

        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorPedido.ParaDto(pedido);
    }

    private static void ValidarEntregaExigida(Domain.Entities.Entrega? entrega, StatusPedido novoStatus)
    {
        switch (novoStatus)
        {
            case StatusPedido.EmRota when entrega is null:
                throw new RegraNegocioException(
                    "O pedido só pode entrar em rota quando houver entrega vinculada.");

            case StatusPedido.EmRota when entrega!.Status != StatusEntrega.EmRota:
                throw new RegraNegocioException(
                    "A entrega precisa estar em rota antes de o pedido entrar em rota.");

            case StatusPedido.Entregue when entrega is null:
                throw new RegraNegocioException(
                    "O pedido só pode ser entregue quando houver entrega vinculada.");

            case StatusPedido.Entregue when entrega!.Status != StatusEntrega.Entregue:
                throw new RegraNegocioException(
                    "A entrega precisa estar concluída antes de o pedido ser entregue.");
        }
    }
}
