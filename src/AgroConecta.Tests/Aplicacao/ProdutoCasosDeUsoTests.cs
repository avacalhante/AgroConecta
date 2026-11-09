using AgroConecta.Application.Common;
using AgroConecta.Application.Produtos;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class ProdutoCasosDeUsoTests
{
    [Fact]
    public async Task Listar_produtos_normaliza_paginacao_e_mapeia_dtos()
    {
        var repositorio = new Mock<IProdutoRepositorio>();
        repositorio.Setup(cenario => cenario.ListarAsync(1, 100, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<Produto>(
                [
                    new Produto
                    {
                        Id = 1,
                        Nome = "Tomate italiano",
                        Categoria = "Hortifrúti",
                        Unidade = "kg",
                        PrecoBase = 8.50m,
                        Ativo = true,
                    },
                ],
                total: 1,
                pagina: 1,
                tamanhoPagina: 100));

        var casoDeUso = new ListarProdutosCasoDeUso(repositorio.Object);

        var resultado = await casoDeUso.ExecutarAsync(new ListarProdutosConsulta(0, 500, true));

        Assert.Single(resultado.Itens);
        Assert.Equal(1, resultado.Pagina);
        Assert.Equal(100, resultado.TamanhoPagina);
        Assert.Equal("Tomate italiano", resultado.Itens.First().Nome);
    }

    [Fact]
    public async Task Criar_produto_persiste_e_retorna_dto()
    {
        var repositorio = new Mock<IProdutoRepositorio>();
        var unidadeDeTrabalho = new Mock<IUnidadeDeTrabalho>();

        var casoDeUso = new CriarProdutoCasoDeUso(
            repositorio.Object,
            unidadeDeTrabalho.Object,
            new CriarProdutoComandoValidator());

        var produto = await casoDeUso.ExecutarAsync(
            new CriarProdutoComando("  Milho verde ", " Grãos ", " kg ", 5.40m));

        Assert.Equal("Milho verde", produto.Nome);
        Assert.Equal("Grãos", produto.Categoria);
        Assert.True(produto.Ativo);

        repositorio.Verify(
            cenario => cenario.AdicionarAsync(
                It.Is<Produto>(entidade => entidade.Nome == "Milho verde"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        unidadeDeTrabalho.Verify(
            unidade => unidade.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Criar_produto_rejeita_preco_negativo()
    {
        var casoDeUso = new CriarProdutoCasoDeUso(
            Mock.Of<IProdutoRepositorio>(),
            Mock.Of<IUnidadeDeTrabalho>(),
            new CriarProdutoComandoValidator());

        await Assert.ThrowsAsync<ValidacaoException>(
            () => casoDeUso.ExecutarAsync(new CriarProdutoComando("Milho", "Grãos", "kg", -1m)));
    }
}
