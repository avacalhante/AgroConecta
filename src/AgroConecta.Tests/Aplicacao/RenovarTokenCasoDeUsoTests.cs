using AgroConecta.Application.Auth;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using Moq;

namespace AgroConecta.Tests.Aplicacao;

public class RenovarTokenCasoDeUsoTests
{
    private readonly Mock<IUsuarioRepositorio> _usuarios = new();
    private readonly Mock<IGeradorToken> _geradorToken = new();

    private RenovarTokenCasoDeUso CriarCasoDeUso() =>
        new(_usuarios.Object, _geradorToken.Object);

    private static Usuario UsuarioMotorista() => new()
    {
        Id = 5,
        Nome = "Marcos Motorista",
        Email = "motorista@agroconecta.local",
        Papel = PapelUsuario.Motorista,
        Ativo = true,
    };

    [Fact]
    public async Task Refresh_valido_renova_os_tokens()
    {
        var usuario = UsuarioMotorista();
        _geradorToken.Setup(gerador => gerador.ObterUsuarioIdDoRefreshToken("refresh-ok"))
            .Returns(usuario.Id);
        _usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _geradorToken.Setup(gerador => gerador.Gerar(usuario))
            .Returns(new TokenPar("novo-acesso", "novo-refresh", 900));

        var resultado = await CriarCasoDeUso().ExecutarAsync(new RenovarTokenComando("refresh-ok"));

        Assert.Equal("novo-acesso", resultado.AccessToken);
        Assert.Equal("novo-refresh", resultado.RefreshToken);
        Assert.Equal(usuario.Id, resultado.Usuario.Id);
    }

    [Fact]
    public async Task Refresh_invalido_retorna_nao_autorizado()
    {
        _geradorToken.Setup(gerador => gerador.ObterUsuarioIdDoRefreshToken(It.IsAny<string>()))
            .Returns((int?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CriarCasoDeUso().ExecutarAsync(new RenovarTokenComando("invalido")));
    }

    [Fact]
    public async Task Refresh_vazio_retorna_nao_autorizado()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CriarCasoDeUso().ExecutarAsync(new RenovarTokenComando(" ")));
    }

    [Fact]
    public async Task Usuario_removido_retorna_nao_autorizado()
    {
        _geradorToken.Setup(gerador => gerador.ObterUsuarioIdDoRefreshToken("refresh-ok"))
            .Returns(99);
        _usuarios.Setup(repositorio => repositorio.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CriarCasoDeUso().ExecutarAsync(new RenovarTokenComando("refresh-ok")));
    }
}
