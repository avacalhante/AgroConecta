using AgroConecta.Domain.Entities;

namespace AgroConecta.Application.Auth;

/// <summary>Emissão e leitura de tokens de acesso e renovação.</summary>
public interface IGeradorToken
{
    /// <summary>Gera um par de tokens para o usuário autenticado.</summary>
    TokenPar Gerar(Usuario usuario);

    /// <summary>
    /// Lê o identificador do usuário a partir de um refresh token válido.
    /// Retorna <c>null</c> quando o token é inválido, expirado ou de outro tipo.
    /// </summary>
    int? ObterUsuarioIdDoRefreshToken(string refreshToken);
}
