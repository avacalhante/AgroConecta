using AgroConecta.Application.Auth;
using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class AutenticarCasoDeUsoTests
{
    private readonly Mock<IUsuarioRepositorio> _usuarios = new();
    private readonly Mock<ISenhaHasher> _senhaHasher = new();
    private readonly Mock<IGeradorToken> _geradorToken = new();

    private static Usuario UsuarioComprador() => new()
    {
        Id = 3,
        Nome = "Carla Compradora",
        Email = "comprador@agroconecta.local",
        Papel = PapelUsuario.Comprador,
        Ativo = true,
    };

    private AutenticarCasoDeUso CriarCasoDeUso() => new(
        _usuarios.Object,
        _senhaHasher.Object,
        _geradorToken.Object,
        new LoginComandoValidator());

    [Fact]
    public async Task Login_valido_retorna_tokens_e_dados_do_usuario()
    {
        var usuario = UsuarioComprador();
        _usuarios.Setup(repositorio => repositorio.ObterPorEmailAsync(
                "comprador@agroconecta.local",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _senhaHasher.Setup(hasher => hasher.Verificar(usuario, "Comprador@123"))
            .Returns(true);
        _geradorToken.Setup(gerador => gerador.Gerar(usuario))
            .Returns(new TokenPar("acesso", "renovacao", 900));

        var resultado = await CriarCasoDeUso().ExecutarAsync(
            new LoginComando("  Comprador@AgroConecta.local ", "Comprador@123"));

        Assert.Equal("acesso", resultado.AccessToken);
        Assert.Equal("renovacao", resultado.RefreshToken);
        Assert.Equal(900, resultado.ExpiresIn);
        Assert.Equal(usuario.Id, resultado.Usuario.Id);
        Assert.Equal(PapelUsuario.Comprador, resultado.Usuario.Papel);
    }

    [Fact]
    public async Task Email_inexistente_retorna_nao_autorizado()
    {
        _usuarios.Setup(repositorio => repositorio.ObterPorEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CriarCasoDeUso().ExecutarAsync(
                new LoginComando("ninguem@agroconecta.local", "Qualquer@123")));

        _geradorToken.Verify(
            gerador => gerador.Gerar(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task Senha_errada_retorna_nao_autorizado()
    {
        var usuario = UsuarioComprador();
        _usuarios.Setup(repositorio => repositorio.ObterPorEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _senhaHasher.Setup(hasher => hasher.Verificar(usuario, It.IsAny<string>()))
            .Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CriarCasoDeUso().ExecutarAsync(
                new LoginComando(usuario.Email, "SenhaErrada@123")));
    }

    [Fact]
    public async Task Usuario_inativo_retorna_nao_autorizado()
    {
        var usuario = UsuarioComprador();
        usuario.Ativo = false;

        _usuarios.Setup(repositorio => repositorio.ObterPorEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _senhaHasher.Setup(hasher => hasher.Verificar(usuario, It.IsAny<string>()))
            .Returns(true);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CriarCasoDeUso().ExecutarAsync(
                new LoginComando(usuario.Email, "Comprador@123")));
    }

    [Fact]
    public async Task Comando_invalido_falha_antes_de_consultar_o_repositorio()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => CriarCasoDeUso().ExecutarAsync(new LoginComando("", "123")));

        Assert.Contains(nameof(LoginComando.Email), excecao.Erros.Keys);
        Assert.Contains(nameof(LoginComando.Senha), excecao.Erros.Keys);
        _usuarios.VerifyNoOtherCalls();
    }
}
