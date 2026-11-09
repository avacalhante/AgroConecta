using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Produtores;

public record AprovarProdutorComando(int ProdutorId, bool Aprovar, int AprovadorId);

public class AprovarProdutorCasoDeUso(
    IProdutorRepositorio produtores,
    IUsuarioRepositorio usuarios,
    IUnidadeDeTrabalho unidadeDeTrabalho)
    : ICasoDeUso<AprovarProdutorComando, bool>
{
    public async Task<bool> ExecutarAsync(
        AprovarProdutorComando entrada,
        CancellationToken cancellationToken = default)
    {
        var produtor = await produtores.ObterPorIdAsync(entrada.ProdutorId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Produtor não encontrado.");

        var aprovador = await usuarios.ObterPorIdAsync(entrada.AprovadorId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Usuário aprovador não encontrado.");

        if (aprovador.Papel != PapelUsuario.Admin)
        {
            throw new AcessoNegadoException("Apenas administradores podem aprovar cadastros.");
        }

        produtor.StatusCadastro = entrada.Aprovar
            ? StatusCadastro.Aprovado
            : StatusCadastro.Rejeitado;
        produtor.AprovadoEm = DateTime.UtcNow;
        produtor.AprovadoPorUsuarioId = aprovador.Id;

        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
