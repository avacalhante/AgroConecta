using AgroConecta.Application.Common;
using AgroConecta.Application.Produtos;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using FluentValidation;

namespace AgroConecta.Application.Produtos;

public class ListarProdutosCasoDeUso(IProdutoRepositorio repositorio)
    : ICasoDeUso<ListarProdutosConsulta, ResultadoPaginado<ProdutoDto>>
{
    public async Task<ResultadoPaginado<ProdutoDto>> ExecutarAsync(
        ListarProdutosConsulta entrada,
        CancellationToken cancellationToken = default)
    {
        var pagina = Math.Max(entrada.Pagina, 1);
        var tamanhoPagina = Math.Clamp(entrada.TamanhoPagina, 1, 100);

        var resultado = await repositorio.ListarAsync(
            pagina,
            tamanhoPagina,
            entrada.Ativo,
            cancellationToken);

        return new ResultadoPaginado<ProdutoDto>(
            resultado.Itens.Select(ParaDto).ToArray(),
            resultado.Total,
            resultado.Pagina,
            resultado.TamanhoPagina);
    }

    internal static ProdutoDto ParaDto(Produto produto) => new(
        produto.Id,
        produto.Nome,
        produto.Categoria,
        produto.Unidade,
        produto.PrecoBase,
        produto.Ativo);
}

public class CriarProdutoCasoDeUso(
    IProdutoRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarProdutoComando> validador)
    : ICasoDeUso<CriarProdutoComando, ProdutoDto>
{
    public async Task<ProdutoDto> ExecutarAsync(
        CriarProdutoComando entrada,
        CancellationToken cancellationToken = default)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var produto = new Produto
        {
            Nome = entrada.Nome.Trim(),
            Categoria = entrada.Categoria.Trim(),
            Unidade = entrada.Unidade.Trim(),
            PrecoBase = entrada.PrecoBase,
            Ativo = true,
        };

        await repositorio.AdicionarAsync(produto, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return ListarProdutosCasoDeUso.ParaDto(produto);
    }
}
