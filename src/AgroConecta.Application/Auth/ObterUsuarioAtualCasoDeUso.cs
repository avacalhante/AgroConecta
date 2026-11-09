using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Auth;

public class ObterUsuarioAtualCasoDeUso(IUsuarioRepositorio usuarios)
    : ICasoDeUso<int, UsuarioDto>
{
    public async Task<UsuarioDto> ExecutarAsync(
        int entrada,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        return MapeadorUsuario.ParaDto(usuario);
    }
}
