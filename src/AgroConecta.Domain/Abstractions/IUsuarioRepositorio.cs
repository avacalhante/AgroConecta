using AgroConecta.Domain.Entities;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência de usuários e perfis.</summary>
public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
