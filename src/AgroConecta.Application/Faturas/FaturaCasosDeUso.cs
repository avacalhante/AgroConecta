using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Faturas;

public record FaturaDto(
    int Id,
    int PedidoId,
    string Numero,
    decimal Valor,
    StatusFatura Status,
    DateTime? EmitidaEm,
    DateTime? PagoEm);

public record GerarFaturaComando(int PedidoId);

public static class MapeadorFatura
{
    public static FaturaDto ParaDto(Fatura fatura) => new(
        fatura.Id,
        fatura.PedidoId,
        fatura.Numero,
        fatura.Valor,
        fatura.Status,
        fatura.EmitidaEm,
        fatura.PagoEm);
}

public class GerarFaturaCasoDeUso(
    IFaturaRepositorio faturas,
    IPedidoRepositorio pedidos,
    IUnidadeDeTrabalho unidadeDeTrabalho)
    : ICasoDeUso<GerarFaturaComando, FaturaDto>
{
    public async Task<FaturaDto> ExecutarAsync(
        GerarFaturaComando entrada,
        CancellationToken cancellationToken = default)
    {
        var pedido = await pedidos.ObterPorIdAsync(entrada.PedidoId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Pedido não encontrado.");

        var existente = await faturas.ObterPorPedidoAsync(entrada.PedidoId, cancellationToken);

        if (existente is not null)
        {
            return MapeadorFatura.ParaDto(existente);
        }

        if (pedido.Status is StatusPedido.Rascunho or StatusPedido.Cancelado)
        {
            throw new RegraNegocioException(
                "Não é possível faturar um pedido em rascunho ou cancelado.");
        }

        var fatura = new Fatura
        {
            PedidoId = pedido.Id,
            Pedido = pedido,
            Numero = $"FAT-{pedido.DataPedido:yyyy}-{pedido.Id:D5}",
            Valor = pedido.ValorTotal,
            Status = StatusFatura.Emitida,
            EmitidaEm = DateTime.UtcNow,
        };

        await faturas.AdicionarAsync(fatura, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorFatura.ParaDto(fatura);
    }
}

public class ListarFaturasCasoDeUso(IFaturaRepositorio faturas)
    : ICasoDeUso<bool, IReadOnlyCollection<FaturaDto>>
{
    public async Task<IReadOnlyCollection<FaturaDto>> ExecutarAsync(
        bool entrada,
        CancellationToken cancellationToken = default)
    {
        var resultado = await faturas.ListarAsync(cancellationToken);
        return resultado.Select(MapeadorFatura.ParaDto).ToArray();
    }
}
