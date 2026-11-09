using AgroConecta.Application.Common;
using AgroConecta.Application.Pedidos;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Pedidos;

public class ObterPedidoCasoDeUso(IPedidoRepositorio repositorio) : ICasoDeUso<int, PedidoDto>
{
    public async Task<PedidoDto> ExecutarAsync(int entrada, CancellationToken cancellationToken = default)
    {
        var pedido = await repositorio.ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Pedido não encontrado.");

        return MapeadorPedido.ParaDto(pedido);
    }
}
