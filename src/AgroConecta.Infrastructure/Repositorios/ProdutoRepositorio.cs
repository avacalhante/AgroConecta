using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class ProdutoRepositorio(AgroConectaDbContext contexto) : IProdutoRepositorio
{
    public async Task<ResultadoPaginado<Produto>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        bool? ativo = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Produtos.AsNoTracking();

        if (ativo is not null)
        {
            consulta = consulta.Where(produto => produto.Ativo == ativo);
        }

        var total = await consulta.CountAsync(cancellationToken);
        var itens = await consulta
            .OrderBy(produto => produto.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Produto>(itens, total, pagina, tamanhoPagina);
    }

    public Task<Produto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Produtos.FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);

    public async Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default) =>
        await contexto.Produtos.AddAsync(produto, cancellationToken);
}
