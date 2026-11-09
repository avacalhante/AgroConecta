using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class UsuarioRepositorio(AgroConectaDbContext contexto) : IUsuarioRepositorio
{
    public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Usuarios
            .Include(usuario => usuario.Produtor)
            .Include(usuario => usuario.Motorista)
            .FirstOrDefaultAsync(usuario => usuario.Id == id, cancellationToken);

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default) =>
        contexto.Usuarios
            .Include(usuario => usuario.Produtor)
            .Include(usuario => usuario.Motorista)
            .FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
        await contexto.Usuarios.AddAsync(usuario, cancellationToken);
}
