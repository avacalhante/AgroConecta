using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class RotaRepositorio(AgroConectaDbContext contexto) : IRotaRepositorio
{
    public Task<Rota?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        contexto.Rotas
            .Include(rota => rota.Motorista)
            .ThenInclude(motorista => motorista.Usuario)
            .Include(rota => rota.Veiculo)
            .Include(rota => rota.Entregas)
            .ThenInclude(entrega => entrega.Pedido)
            .FirstOrDefaultAsync(rota => rota.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Rota>> ListarAsync(CancellationToken cancellationToken = default) =>
        await contexto.Rotas
            .AsNoTracking()
            .Include(rota => rota.Motorista)
            .ThenInclude(motorista => motorista.Usuario)
            .Include(rota => rota.Veiculo)
            .Include(rota => rota.Entregas)
            .ThenInclude(entrega => entrega.Pedido)
            .OrderByDescending(rota => rota.Data)
            .ThenBy(rota => rota.Id)
            .ToListAsync(cancellationToken);

    public async Task AdicionarAsync(Rota rota, CancellationToken cancellationToken = default) =>
        await contexto.Rotas.AddAsync(rota, cancellationToken);

    public Task<bool> MotoristaAtivoExisteAsync(
        int motoristaId,
        CancellationToken cancellationToken = default) =>
        contexto.Motoristas.AnyAsync(
            motorista => motorista.Id == motoristaId && motorista.Ativo,
            cancellationToken);

    public Task<bool> VeiculoAtivoExisteAsync(
        int veiculoId,
        CancellationToken cancellationToken = default) =>
        contexto.Veiculos.AnyAsync(
            veiculo => veiculo.Id == veiculoId && veiculo.Ativo,
            cancellationToken);

    public Task<bool> PedidoPossuiEntregaAsync(
        int pedidoId,
        CancellationToken cancellationToken = default) =>
        contexto.Entregas.AnyAsync(entrega => entrega.PedidoId == pedidoId, cancellationToken);
}
