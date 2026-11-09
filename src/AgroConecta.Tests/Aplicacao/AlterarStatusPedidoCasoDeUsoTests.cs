using AgroConecta.Application.Pedidos;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class AlterarStatusPedidoCasoDeUsoTests
{
    private readonly Mock<IPedidoRepositorio> _pedidos = new();
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho = new();

    private AlterarStatusPedidoCasoDeUso CriarCasoDeUso() =>
        new(_pedidos.Object, _unidadeDeTrabalho.Object);

    private static Pedido PedidoEmSeparacao()
    {
        var pedido = new Pedido { EnderecoEntrega = "Rua de Teste, 10" };
        pedido.AlterarStatus(StatusPedido.AguardandoAprovacao);
        pedido.AlterarStatus(StatusPedido.Aprovado);
        pedido.AlterarStatus(StatusPedido.EmSeparacao);
        return pedido;
    }

    [Fact]
    public async Task Altera_status_quando_a_transicao_e_valida()
    {
        var pedido = PedidoEmSeparacao();
        _pedidos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        var resultado = await CriarCasoDeUso().ExecutarAsync(
            new AlterarStatusPedidoComando(1, StatusPedido.Cancelado));

        Assert.Equal(StatusPedido.Cancelado, resultado.Status);
        _unidadeDeTrabalho.Verify(
            unidade => unidade.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Rejeita_pedido_inexistente()
    {
        _pedidos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => CriarCasoDeUso().ExecutarAsync(new AlterarStatusPedidoComando(1, StatusPedido.Aprovado)));
    }

    [Fact]
    public async Task Nao_coloca_pedido_em_rota_sem_entrega_em_rota()
    {
        var pedido = PedidoEmSeparacao();
        _pedidos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => CriarCasoDeUso().ExecutarAsync(new AlterarStatusPedidoComando(1, StatusPedido.EmRota)));
    }

    [Fact]
    public async Task Coloca_pedido_em_rota_com_entrega_em_rota()
    {
        var pedido = PedidoEmSeparacao();
        var entrega = new Entrega { Pedido = pedido, DataPrevista = new DateOnly(2026, 11, 5) };
        entrega.AlterarStatus(StatusEntrega.EmRota);
        pedido.Entrega = entrega;

        _pedidos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        var resultado = await CriarCasoDeUso().ExecutarAsync(
            new AlterarStatusPedidoComando(1, StatusPedido.EmRota));

        Assert.Equal(StatusPedido.EmRota, resultado.Status);
    }

    [Fact]
    public async Task Nao_entrega_pedido_com_entrega_pendente()
    {
        var pedido = PedidoEmSeparacao();
        pedido.AlterarStatus(StatusPedido.EmRota);
        pedido.Entrega = new Entrega { Pedido = pedido, DataPrevista = new DateOnly(2026, 11, 5) };

        _pedidos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => CriarCasoDeUso().ExecutarAsync(new AlterarStatusPedidoComando(1, StatusPedido.Entregue)));
    }
}
