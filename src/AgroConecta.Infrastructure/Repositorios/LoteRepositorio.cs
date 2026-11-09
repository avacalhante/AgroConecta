using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Common;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class LoteRepositorio(AgroConectaDbContext contexto) : ILoteRepositorio
{
    public Task<LoteColheita?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        ConsultaCompleta().FirstOrDefaultAsync(lote => lote.Id == id, cancellationToken);

    public Task<LoteColheita?> ObterPorCodigoRastreioAsync(
        Guid codigoRastreio,
        CancellationToken cancellationToken = default) =>
        ConsultaCompleta().FirstOrDefaultAsync(
            lote => lote.CodigoRastreio == codigoRastreio,
            cancellationToken);

    public Task<bool> ExisteCodigoRastreioAsync(
        Guid codigoRastreio,
        CancellationToken cancellationToken = default) =>
        contexto.LotesColheita.AnyAsync(
            lote => lote.CodigoRastreio == codigoRastreio,
            cancellationToken);

    public async Task<ResultadoPaginado<LoteColheita>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        StatusLote? status = null,
        int? produtorId = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.LotesColheita
            .AsNoTracking()
            .Include(lote => lote.Produto)
            .AsQueryable();

        if (status is not null)
        {
            consulta = consulta.Where(lote => lote.Status == status);
        }

        if (produtorId is not null)
        {
            consulta = consulta.Where(lote => lote.ProdutorId == produtorId);
        }

        var total = await consulta.CountAsync(cancellationToken);
        var itens = await consulta
            .OrderByDescending(lote => lote.DataColheita)
            .ThenByDescending(lote => lote.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<LoteColheita>(itens, total, pagina, tamanhoPagina);
    }

    public async Task AdicionarAsync(LoteColheita lote, CancellationToken cancellationToken = default) =>
        await contexto.LotesColheita.AddAsync(lote, cancellationToken);

    private IQueryable<LoteColheita> ConsultaCompleta() =>
        contexto.LotesColheita
            .Include(lote => lote.Produto)
            .Include(lote => lote.Produtor)
            .ThenInclude(produtor => produtor.Usuario);
}
