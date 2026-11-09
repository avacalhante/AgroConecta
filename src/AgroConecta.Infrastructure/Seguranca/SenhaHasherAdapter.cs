using AgroConecta.Application.Auth;
using AgroConecta.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AgroConecta.Infrastructure.Seguranca;

/// <summary>Adapta o <see cref="PasswordHasher{TUser}"/> do ASP.NET Core ao contrato da Application.</summary>
public class SenhaHasherAdapter : ISenhaHasher
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public bool Verificar(Usuario usuario, string senha) =>
        _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha)
            != PasswordVerificationResult.Failed;
}
