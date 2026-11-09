using AgroConecta.Application.Common;
using AgroConecta.Application.Pedidos;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class CriarPedidoCasoDeUsoTests
{
    private readonly Mock<IPedidoRepositorio> _pedidos = new();
    private readonly Mock<IProdutoRepositorio> _produtos = new();
    private readonly Mock<ILoteRepositorio> _lotes = new();
    private readonly Mock<IUsuarioRepositorio> _usuarios = new();
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho = new();

    private CriarPedidoCasoDeUso CriarCasoDeUso() => new(
        _pedidos.Object,
        _produtos.Object,
        _lotes.Object,
        _usuarios.Object,
        _unidadeDeTrabalho.Object,
        new CriarPedidoComandoValidator());

    private void PrepararCenario(
        PapelUsuario papel = PapelUsuario.Comprador,
        StatusLote statusLote = StatusLote.Disponivel)
    {
        var comprador = new Usuario
        {
            Id = 10,
            Nome = "Carla Compradora",
            Email = "comprador@teste.local",
            Papel = papel,
        };

        var produto = new Produto
        {
            Id = 1,
            Nome = "Tomate italiano",
            Categoria = "Hortifrúti",
            Unidade = "kg",
            PrecoBase = 8.50m,
        };

        var lote = new LoteColheita
        {
            Id = 5,
            ProdutoId = 1,
            Produto = produto,
            CodigoRastreio = Guid.NewGuid(),
            DataColheita = new DateOnly(2026, 11, 1),
            Quantidade = 100m,
        };

        if (statusLote == StatusLote.Reservado)
        {
            lote.Reservar();
        }

        _usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comprador);
        _produtos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _lotes.Setup(repositorio => repositorio.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lote);
    }

    private static CriarPedidoComando ComandoValido(int? loteId = 5) => new(
        10,
        "Rua das Acácias, 120",
        [new CriarItemPedidoComando(1, loteId, 10m, 8.50m)]);

    [Fact]
    public async Task Cria_pedido_com_total_e_status_aguardando_aprovacao()
    {
        PrepararCenario();

        var pedido = await CriarCasoDeUso().ExecutarAsync(ComandoValido());

        Assert.Equal(85.00m, pedido.ValorTotal);
        Assert.Equal(StatusPedido.AguardandoAprovacao, pedido.Status);
        Assert.Single(pedido.Itens);
        _pedidos.Verify(
            repositorio => repositorio.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unidadeDeTrabalho.Verify(
            unidade => unidade.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Reserva_o_lote_informado()
    {
        PrepararCenario();

        await CriarCasoDeUso().ExecutarAsync(ComandoValido());

        var lote = await _lotes.Object.ObterPorIdAsync(5);
        Assert.Equal(StatusLote.Reservado, lote!.Status);
    }

    [Fact]
    public async Task Rejeita_comprador_inexistente()
    {
        _usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => CriarCasoDeUso().ExecutarAsync(ComandoValido()));
    }

    [Fact]
    public async Task Rejeita_usuario_que_nao_e_comprador()
    {
        PrepararCenario(PapelUsuario.Produtor);

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => CriarCasoDeUso().ExecutarAsync(ComandoValido()));
    }

    [Fact]
    public async Task Rejeita_lote_indisponivel()
    {
        PrepararCenario(statusLote: StatusLote.Reservado);

        await Assert.ThrowsAsync<RegraNegocioException>(
            () => CriarCasoDeUso().ExecutarAsync(ComandoValido()));
    }

    [Fact]
    public async Task Rejeita_produto_inexistente()
    {
        PrepararCenario();
        _produtos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => CriarCasoDeUso().ExecutarAsync(ComandoValido()));
    }

    [Fact]
    public async Task Rejeita_comando_invalido_antes_de_persistir()
    {
        var comando = new CriarPedidoComando(10, " ", []);

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => CriarCasoDeUso().ExecutarAsync(comando));

        Assert.Contains(nameof(CriarPedidoComando.EnderecoEntrega), excecao.Erros.Keys);
        Assert.Contains(nameof(CriarPedidoComando.Itens), excecao.Erros.Keys);
        _pedidos.VerifyNoOtherCalls();
    }
}
