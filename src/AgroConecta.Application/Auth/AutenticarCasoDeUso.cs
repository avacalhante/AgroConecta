using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using FluentValidation;

namespace AgroConecta.Application.Auth;

public class AutenticarCasoDeUso(
    IUsuarioRepositorio usuarios,
    ISenhaHasher senhaHasher,
    IGeradorToken geradorToken,
    IValidator<LoginComando> validador)
    : ICasoDeUso<LoginComando, LoginResultado>
{
    public async Task<LoginResultado> ExecutarAsync(
        LoginComando entrada,
        CancellationToken cancellationToken = default)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var email = entrada.Email.Trim().ToLowerInvariant();
        var usuario = await usuarios.ObterPorEmailAsync(email, cancellationToken);

        if (usuario is null || !senhaHasher.Verificar(usuario, entrada.Senha))
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
        }

        if (!usuario.Ativo)
        {
            throw new UnauthorizedAccessException("Usuário inativo.");
        }

        var tokens = geradorToken.Gerar(usuario);

        return LoginResultado.De(tokens, MapeadorUsuario.ParaDto(usuario));
    }
}

public static class MapeadorUsuario
{
    public static UsuarioDto ParaDto(Usuario usuario) => new(
        usuario.Id,
        usuario.Nome,
        usuario.Email,
        usuario.Papel,
        usuario.Ativo);
}
