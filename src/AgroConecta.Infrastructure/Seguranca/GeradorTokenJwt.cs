using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgroConecta.Application.Auth;
using AgroConecta.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AgroConecta.Infrastructure.Seguranca;

/// <summary>
/// Emite access tokens e refresh tokens assinados com HMAC-SHA256.
/// O tipo do token vai na claim <c>token_type</c>, impedindo que um refresh
/// token seja aceito como token de acesso.
/// </summary>
public class GeradorTokenJwt(JwtConfiguracao configuracao) : IGeradorToken
{
    public const string ClaimTipoToken = "token_type";
    public const string TipoAcesso = "access";
    public const string TipoRenovacao = "refresh";

    public TokenPar Gerar(Usuario usuario)
    {
        var accessToken = CriarToken(
            usuario,
            TipoAcesso,
            TimeSpan.FromMinutes(configuracao.AccessTokenMinutos));

        var refreshToken = CriarToken(
            usuario,
            TipoRenovacao,
            TimeSpan.FromDays(configuracao.RefreshTokenDias));

        return new TokenPar(
            accessToken,
            refreshToken,
            (int)TimeSpan.FromMinutes(configuracao.AccessTokenMinutos).TotalSeconds);
    }

    public int? ObterUsuarioIdDoRefreshToken(string refreshToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(
                refreshToken,
                CriarParametrosValidacao(lifetime: true),
                out _);

            var tipo = principal.FindFirstValue(ClaimTipoToken);

            if (tipo != TipoRenovacao)
            {
                return null;
            }

            var identificador = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(identificador, out var usuarioId) ? usuarioId : null;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private string CriarToken(Usuario usuario, string tipo, TimeSpan validade)
    {
        var agora = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Papel.ToString()),
            new(ClaimTipoToken, tipo),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var credenciais = new SigningCredentials(
            CriarChave(configuracao.Chave),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuracao.Issuer,
            audience: configuracao.Audience,
            claims: claims,
            notBefore: agora,
            expires: agora.Add(validade),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static SymmetricSecurityKey CriarChave(string chave)
    {
        if (string.IsNullOrWhiteSpace(chave) || Encoding.UTF8.GetByteCount(chave) < 32)
        {
            throw new InvalidOperationException(
                "A chave JWT deve ter pelo menos 32 bytes. Configure a seção 'Jwt:Chave'.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
    }

    private TokenValidationParameters CriarParametrosValidacao(bool lifetime) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = configuracao.Issuer,
        ValidateAudience = true,
        ValidAudience = configuracao.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = CriarChave(configuracao.Chave),
        ValidateLifetime = lifetime,
        ClockSkew = TimeSpan.FromSeconds(30),
    };
}
