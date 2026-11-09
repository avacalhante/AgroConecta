using AgroConecta.Application.Common;
using AgroConecta.Application.Lotes;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class LoteCasosDeUsoTests
{
    private static Produtor ProdutorPaulo() => new()
    {
        Id = 1,
        NomePropriedade = "Sítio Boa Esperança",
        Usuario = new Usuario
        {
            Id = 2,
            Nome = "Paulo Produtor",
            Email = "produtor@agroconecta.local",
            Papel = PapelUsuario.Produtor,
        },
    };

    private static CriarLoteComando ComandoValido(Guid? codigo = null) => new(
        UsuarioId: 2,
        ProdutoId: 1,
        CodigoRastreio: codigo ?? Guid.NewGuid(),
        DataColheita: new DateOnly(2026, 11, 1),
        Quantidade: 50m);

    private static CriarLoteCasoDeUso CriarCasoDeUso(
        Mock<ILoteRepositorio>? lotes = null,
        Mock<IProdutoRepositorio>? produtos = null,
        Mock<IUsuarioRepositorio>? usuarios = null,
        Mock<IUnidadeDeTrabalho>? unidade = null)
    {
        return new CriarLoteCasoDeUso(
            (lotes ?? new Mock<ILoteRepositorio>()).Object,
            (produtos ?? new Mock<IProdutoRepositorio>()).Object,
            (usuarios ?? new Mock<IUsuarioRepositorio>()).Object,
            (unidade ?? new Mock<IUnidadeDeTrabalho>()).Object,
            new CriarLoteComandoValidator());
    }

    [Fact]
    public async Task Cria_lote_para_o_produtor_autenticado()
    {
        var produtor = ProdutorPaulo();
        var usuario = produtor.Usuario;
        usuario.Produtor = produtor;

        var produtos = new Mock<IProdutoRepositorio>();
        produtos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Produto { Id = 1, Nome = "Tomate italiano" });

        var lotes = new Mock<ILoteRepositorio>();
        lotes.Setup(repositorio => repositorio.ExisteCodigoRastreioAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var unidade = new Mock<IUnidadeDeTrabalho>();

        var lote = await CriarCasoDeUso(lotes, produtos, usuarios, unidade)
            .ExecutarAsync(ComandoValido());

        Assert.Equal(produtor.Id, lote.ProdutorId);
        Assert.Equal("Tomate italiano", lote.ProdutoNome);
        Assert.Equal(StatusLote.Disponivel, lote.Status);
        lotes.Verify(repositorio => repositorio.AdicionarAsync(
            It.IsAny<LoteColheita>(),
            It.IsAny<CancellationToken>()), Times.Once);
        unidade.Verify(
            trabalho => trabalho.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Codigo_de_rastreio_duplicado_retorna_conflito()
    {
        var produtor = ProdutorPaulo();
        var usuario = produtor.Usuario;
        usuario.Produtor = produtor;

        var produtos = new Mock<IProdutoRepositorio>();
        produtos.Setup(repositorio => repositorio.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Produto { Id = 1, Nome = "Tomate italiano" });

        var lotes = new Mock<ILoteRepositorio>();
        lotes.Setup(repositorio => repositorio.ExisteCodigoRastreioAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        await Assert.ThrowsAsync<ConflitoException>(
            () => CriarCasoDeUso(lotes, produtos, usuarios).ExecutarAsync(ComandoValido()));
    }

    [Fact]
    public async Task Usuario_sem_perfil_de_produtor_recebe_acesso_negado()
    {
        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario
            {
                Id = 2,
                Nome = "Carla Compradora",
                Email = "comprador@agroconecta.local",
                Papel = PapelUsuario.Comprador,
            });

        await Assert.ThrowsAsync<AcessoNegadoException>(
            () => CriarCasoDeUso(usuarios: usuarios).ExecutarAsync(ComandoValido()));
    }

    [Fact]
    public async Task Comando_invalido_nao_persiste()
    {
        var lotes = new Mock<ILoteRepositorio>();

        await Assert.ThrowsAsync<ValidacaoException>(
            () => CriarCasoDeUso(lotes)
                .ExecutarAsync(new CriarLoteComando(2, 0, Guid.Empty, default, 0m)));

        lotes.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Meus_lotes_filtra_pelo_produtor_do_usuario()
    {
        var produtor = ProdutorPaulo();
        var usuario = produtor.Usuario;
        usuario.Produtor = produtor;

        var usuarios = new Mock<IUsuarioRepositorio>();
        usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var lotes = new Mock<ILoteRepositorio>();
        lotes.Setup(repositorio => repositorio.ListarAsync(
                1, 20, null, produtor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<LoteColheita>(
                [],
                0,
                1,
                20));

        var resultado = await new ListarMeusLotesCasoDeUso(lotes.Object, usuarios.Object)
            .ExecutarAsync(new ListarMeusLotesConsulta(2, 1, 20));

        Assert.Empty(resultado.Itens);
        lotes.Verify(repositorio => repositorio.ListarAsync(
            1, 20, null, produtor.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
