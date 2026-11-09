using AgroConecta.Domain.Common;

namespace AgroConecta.Web.Api.Contratos;

/// <summary>Envelope de listagem paginada da API.</summary>
public record PaginaResponse<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int Total)
{
    public static PaginaResponse<T> De(ResultadoPaginado<T> resultado) =>
        new(resultado.Itens, resultado.Pagina, resultado.TamanhoPagina, resultado.Total);
}
