using AgroConecta.Application.Common;
using AgroConecta.Application.Produtos;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Produtos;

public class ObterProdutoCasoDeUso(IProdutoRepositorio repositorio)
    : ICasoDeUso<int, ProdutoDto>
{
    public async Task<ProdutoDto> ExecutarAsync(int entrada, CancellationToken cancellationToken = default)
    {
        var produto = await repositorio.ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Produto não encontrado.");

        return ListarProdutosCasoDeUso.ParaDto(produto);
    }
}
