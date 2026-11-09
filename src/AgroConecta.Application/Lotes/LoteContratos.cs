using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;

namespace AgroConecta.Application.Lotes;

public record LoteDto(
    int Id,
    int ProdutorId,
    int ProdutoId,
    string ProdutoNome,
    Guid CodigoRastreio,
    DateOnly DataColheita,
    decimal Quantidade,
    StatusLote Status,
    DateTime CriadoEm);

public record ListarLotesConsulta(int Pagina, int TamanhoPagina, StatusLote? Status = null);

public record ListarMeusLotesConsulta(int UsuarioId, int Pagina, int TamanhoPagina);

public record CriarLoteComando(
    int UsuarioId,
    int ProdutoId,
    Guid CodigoRastreio,
    DateOnly DataColheita,
    decimal Quantidade);

public static class MapeadorLote
{
    public static LoteDto ParaDto(LoteColheita lote) => new(
        lote.Id,
        lote.ProdutorId,
        lote.ProdutoId,
        lote.Produto?.Nome ?? string.Empty,
        lote.CodigoRastreio,
        lote.DataColheita,
        lote.Quantidade,
        lote.Status,
        lote.CriadoEm);
}
