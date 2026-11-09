using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Auth;

/// <summary>Usuário exposto pela API (nunca inclui hash de senha).</summary>
public record UsuarioDto(int Id, string Nome, string Email, PapelUsuario Papel, bool Ativo);

public record LoginComando(string Email, string Senha);

public record RenovarTokenComando(string RefreshToken);

/// <summary>Par de tokens emitido no login e na renovação.</summary>
public record TokenPar(string AccessToken, string RefreshToken, int ExpiresIn);

public record LoginResultado(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UsuarioDto Usuario)
{
    public static LoginResultado De(TokenPar tokens, UsuarioDto usuario) =>
        new(tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn, usuario);
}
