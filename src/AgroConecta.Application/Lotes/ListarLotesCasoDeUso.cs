using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;

namespace AgroConecta.Application.Lotes;

public class ListarLotesCasoDeUso(ILoteRepositorio repositorio)
    : ICasoDeUso<ListarLotesConsulta, ResultadoPaginado<LoteDto>>
{
    public async Task<ResultadoPaginado<LoteDto>> ExecutarAsync(
        ListarLotesConsulta entrada,
        CancellationToken cancellationToken = default)
    {
        var pagina = Math.Max(entrada.Pagina, 1);
        var tamanhoPagina = Math.Clamp(entrada.TamanhoPagina, 1, 100);

        var resultado = await repositorio.ListarAsync(
            pagina,
            tamanhoPagina,
            entrada.Status,
            produtorId: null,
            cancellationToken);

        return new ResultadoPaginado<LoteDto>(
            resultado.Itens.Select(MapeadorLote.ParaDto).ToArray(),
            resultado.Total,
            resultado.Pagina,
            resultado.TamanhoPagina);
    }
}

public class ListarMeusLotesCasoDeUso(
    ILoteRepositorio lotes,
    IUsuarioRepositorio usuarios)
    : ICasoDeUso<ListarMeusLotesConsulta, ResultadoPaginado<LoteDto>>
{
    public async Task<ResultadoPaginado<LoteDto>> ExecutarAsync(
        ListarMeusLotesConsulta entrada,
        CancellationToken cancellationToken = default)
    {
        var produtor = await ResolvedorProdutor.ObterProdutorAsync(
            usuarios,
            entrada.UsuarioId,
            cancellationToken);

        var pagina = Math.Max(entrada.Pagina, 1);
        var tamanhoPagina = Math.Clamp(entrada.TamanhoPagina, 1, 100);

        var resultado = await lotes.ListarAsync(
            pagina,
            tamanhoPagina,
            status: null,
            produtorId: produtor.Id,
            cancellationToken);

        return new ResultadoPaginado<LoteDto>(
            resultado.Itens.Select(MapeadorLote.ParaDto).ToArray(),
            resultado.Total,
            resultado.Pagina,
            resultado.TamanhoPagina);
    }
}

public class ObterLoteCasoDeUso(ILoteRepositorio repositorio) : ICasoDeUso<int, LoteDto>
{
    public async Task<LoteDto> ExecutarAsync(int entrada, CancellationToken cancellationToken = default)
    {
        var lote = await repositorio.ObterPorIdAsync(entrada, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Lote não encontrado.");

        return MapeadorLote.ParaDto(lote);
    }
}

/// <summary>Resolve o produtor vinculado ao usuário autenticado.</summary>
internal static class ResolvedorProdutor
{
    public static async Task<Domain.Entities.Produtor> ObterProdutorAsync(
        IUsuarioRepositorio usuarios,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObterPorIdAsync(usuarioId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Usuário não encontrado.");

        return usuario.Produtor
            ?? throw new AcessoNegadoException("A operação exige um usuário com perfil de produtor.");
    }
}
