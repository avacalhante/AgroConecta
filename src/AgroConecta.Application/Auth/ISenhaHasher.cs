using AgroConecta.Domain.Entities;

namespace AgroConecta.Application.Auth;

/// <summary>
/// Abstração de verificação de senha. Mantém a camada Application independente
/// da implementação de hash usada pela Infrastructure (DIP).
/// </summary>
public interface ISenhaHasher
{
    bool Verificar(Usuario usuario, string senha);
}
