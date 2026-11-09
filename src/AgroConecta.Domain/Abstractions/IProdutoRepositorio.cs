using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;

namespace AgroConecta.Domain.Abstractions;

/// <summary>Contrato de persistência do catálogo de produtos (ISP).</summary>
public interface IProdutoRepositorio
{
    Task<ResultadoPaginado<Produto>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<Produto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default);
}
