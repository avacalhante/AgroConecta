using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using FluentValidation;

namespace AgroConecta.Application.Lotes;

public class CriarLoteCasoDeUso(
    ILoteRepositorio lotes,
    IProdutoRepositorio produtos,
    IUsuarioRepositorio usuarios,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarLoteComando> validador)
    : ICasoDeUso<CriarLoteComando, LoteDto>
{
    public async Task<LoteDto> ExecutarAsync(
        CriarLoteComando entrada,
        CancellationToken cancellationToken = default)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var produtor = await ResolvedorProdutor.ObterProdutorAsync(
            usuarios,
            entrada.UsuarioId,
            cancellationToken);

        var produto = await produtos.ObterPorIdAsync(entrada.ProdutoId, cancellationToken)
            ?? throw new RecursoNaoEncontradoException("Produto não encontrado.");

        if (await lotes.ExisteCodigoRastreioAsync(entrada.CodigoRastreio, cancellationToken))
        {
            throw new ConflitoException(
                "Já existe um lote com o código de rastreio informado.");
        }

        var lote = new LoteColheita
        {
            ProdutorId = produtor.Id,
            Produtor = produtor,
            ProdutoId = produto.Id,
            Produto = produto,
            CodigoRastreio = entrada.CodigoRastreio,
            DataColheita = entrada.DataColheita,
            Quantidade = entrada.Quantidade,
        };

        await lotes.AdicionarAsync(lote, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorLote.ParaDto(lote);
    }
}
