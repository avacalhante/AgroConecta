using System.IdentityModel.Tokens.Jwt;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Seguranca;

namespace AgroConecta.Tests.Infraestrutura;

public class GeradorTokenJwtTests
{
    private static readonly JwtConfiguracao Configuracao = new()
    {
        Issuer = "AgroConecta.Tests",
        Audience = "AgroConecta.Tests",
        Chave = "chave-de-teste-com-pelo-menos-32-bytes!!",
        AccessTokenMinutos = 15,
        RefreshTokenDias = 7,
    };

    private static Usuario NovoUsuario() => new()
    {
        Id = 7,
        Nome = "Paulo Produtor",
        Email = "produtor@agroconecta.local",
        Papel = PapelUsuario.Produtor,
        Ativo = true,
    };

    [Fact]
    public void Gerar_emite_access_e_refresh_distintos()
    {
        var tokens = new GeradorTokenJwt(Configuracao).Gerar(NovoUsuario());

        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
        Assert.NotEqual(tokens.AccessToken, tokens.RefreshToken);
        Assert.Equal(900, tokens.ExpiresIn);
    }

    [Fact]
    public void Refresh_token_devolve_o_identificador_do_usuario()
    {
        var gerador = new GeradorTokenJwt(Configuracao);
        var tokens = gerador.Gerar(NovoUsuario());

        Assert.Equal(7, gerador.ObterUsuarioIdDoRefreshToken(tokens.RefreshToken));
    }

    [Fact]
    public void Access_token_nao_e_aceito_como_refresh()
    {
        var gerador = new GeradorTokenJwt(Configuracao);
        var tokens = gerador.Gerar(NovoUsuario());

        Assert.Null(gerador.ObterUsuarioIdDoRefreshToken(tokens.AccessToken));
    }

    [Fact]
    public void Token_adulterado_e_rejeitado()
    {
        var gerador = new GeradorTokenJwt(Configuracao);
        var tokens = gerador.Gerar(NovoUsuario());

        Assert.Null(gerador.ObterUsuarioIdDoRefreshToken(tokens.RefreshToken + "x"));
    }

    [Fact]
    public void Access_token_carrega_papel_e_email()
    {
        var gerador = new GeradorTokenJwt(Configuracao);
        var tokens = gerador.Gerar(NovoUsuario());

        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokens.AccessToken);

        Assert.Contains(token.Claims, claim => claim.Value == "Produtor");
        Assert.Contains(token.Claims, claim => claim.Value == "produtor@agroconecta.local");
        Assert.Contains(token.Claims, claim => claim.Type == "token_type" && claim.Value == "access");
    }

    [Fact]
    public void Chave_curta_e_rejeitada()
    {
        Assert.Throws<InvalidOperationException>(
            () => GeradorTokenJwt.CriarChave("curta"));
    }
}
