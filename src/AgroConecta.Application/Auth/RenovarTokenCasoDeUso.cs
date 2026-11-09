using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;

namespace AgroConecta.Application.Auth;

public class RenovarTokenCasoDeUso(
    IUsuarioRepositorio usuarios,
    IGeradorToken geradorToken)
    : ICasoDeUso<RenovarTokenComando, LoginResultado>
{
    public async Task<LoginResultado> ExecutarAsync(
        RenovarTokenComando entrada,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entrada.RefreshToken))
        {
            throw new UnauthorizedAccessException("Refresh token não informado.");
        }

        var usuarioId = geradorToken.ObterUsuarioIdDoRefreshToken(entrada.RefreshToken)
            ?? throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");

        var usuario = await usuarios.ObterPorIdAsync(usuarioId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Usuário do token não encontrado.");

        if (!usuario.Ativo)
        {
            throw new UnauthorizedAccessException("Usuário inativo.");
        }

        var tokens = geradorToken.Gerar(usuario);

        return LoginResultado.De(tokens, MapeadorUsuario.ParaDto(usuario));
    }
}
