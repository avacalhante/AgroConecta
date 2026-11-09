using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Exceptions;
using FluentValidation;

namespace AgroConecta.Application.Pedidos;

public class CriarPedidoCasoDeUso(
    IPedidoRepositorio pedidoRepositorio,
    IProdutoRepositorio produtoRepositorio,
    ILoteRepositorio loteRepositorio,
    IUsuarioRepositorio usuarioRepositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarPedidoComando> validador)
    : ICasoDeUso<CriarPedidoComando, PedidoDto>
{
    public async Task<PedidoDto> ExecutarAsync(
        CriarPedidoComando entrada,
        CancellationToken cancellationToken = default)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var comprador = await usuarioRepositorio.ObterPorIdAsync(entrada.CompradorId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Comprador não encontrado.");

        if (comprador.Papel is not (PapelUsuario.Comprador or PapelUsuario.Admin))
        {
            throw new RegraNegocioException("Apenas compradores podem criar pedidos.");
        }

        var pedido = new Pedido
        {
            Comprador = comprador,
            CompradorId = comprador.Id,
            DataPedido = DateTime.UtcNow,
            EnderecoEntrega = entrada.EnderecoEntrega.Trim(),
        };

        var lotesReservados = new List<LoteColheita>();

        foreach (var itemComando in entrada.Itens)
        {
            var produto = await produtoRepositorio.ObterPorIdAsync(itemComando.ProdutoId, cancellationToken)
                ?? throw new RecursoNaoEncontradoException(
                    $"Produto {itemComando.ProdutoId} não encontrado.");

            LoteColheita? lote = null;

            if (itemComando.LoteId is not null)
            {
                lote = await loteRepositorio.ObterPorIdAsync(itemComando.LoteId.Value, cancellationToken)
                    ?? throw new RecursoNaoEncontradoException(
                        $"Lote {itemComando.LoteId} não encontrado.");

                if (lote.Status != StatusLote.Disponivel)
                {
                    throw new RegraNegocioException(
                        $"O lote {lote.CodigoRastreio} não está disponível para venda.");
                }

                lotesReservados.Add(lote);
            }

            pedido.AdicionarItem(produto, lote, itemComando.Quantidade, itemComando.PrecoUnitario);
        }

        pedido.AlterarStatus(StatusPedido.AguardandoAprovacao);

        foreach (var lote in lotesReservados.Distinct())
        {
            lote.Reservar();
        }

        await pedidoRepositorio.AdicionarAsync(pedido, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorPedido.ParaDto(pedido);
    }
}
