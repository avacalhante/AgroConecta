using AgroConecta.Application.Consultas;
using AgroConecta.Domain.Enums;
using AgroConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgroConecta.Infrastructure.Repositorios;

public class ConsultasWebRepositorio(AgroConectaDbContext contexto) : IConsultasWebRepositorio
{
    public async Task<IReadOnlyList<ProdutorResumo>> ListarProdutoresAsync(
        StatusCadastro? status = null,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Produtores
            .AsNoTracking()
            .AsQueryable();

        if (status is not null)
        {
            consulta = consulta.Where(produtor => produtor.StatusCadastro == status);
        }

        return await consulta
            .OrderBy(produtor => produtor.Usuario.Nome)
            .Select(produtor => new ProdutorResumo(
                produtor.Id,
                produtor.Usuario.Nome,
                produtor.Usuario.Email,
                produtor.Documento,
                produtor.NomePropriedade,
                produtor.Cidade,
                produtor.UF,
                produtor.Comunidade,
                produtor.StatusCadastro,
                produtor.Cooperativa != null ? produtor.Cooperativa.Nome : null,
                produtor.CriadoEm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CooperativaResumo>> ListarCooperativasAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Cooperativas
            .AsNoTracking()
            .OrderBy(cooperativa => cooperativa.Nome)
            .Select(cooperativa => new CooperativaResumo(
                cooperativa.Id,
                cooperativa.Nome,
                cooperativa.Cidade,
                cooperativa.UF,
                cooperativa.Tipo,
                cooperativa.Ativa,
                cooperativa.Produtores.Count))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<FaturaResumo>> ListarFaturasAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Faturas
            .AsNoTracking()
            .OrderByDescending(fatura => fatura.Id)
            .Select(fatura => new FaturaResumo(
                fatura.Id,
                fatura.PedidoId,
                fatura.Numero,
                fatura.Valor,
                fatura.Status,
                fatura.EmitidaEm,
                fatura.PagoEm))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<UsuarioResumo>> ListarUsuariosAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Usuarios
            .AsNoTracking()
            .OrderBy(usuario => usuario.Nome)
            .Select(usuario => new UsuarioResumo(
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Papel,
                usuario.Ativo,
                usuario.CriadoEm))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LogAcessoResumo>> ListarLogsAsync(
        int limite = 100,
        CancellationToken cancellationToken = default) =>
        await contexto.LogsAcesso
            .AsNoTracking()
            .OrderByDescending(log => log.CriadoEm)
            .Take(Math.Clamp(limite, 1, 500))
            .Select(log => new LogAcessoResumo(
                log.Id,
                log.Usuario != null ? log.Usuario.Nome : null,
                log.Acao,
                log.Entidade,
                log.EntidadeId,
                log.IP,
                log.CriadoEm))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<EntregaResumo>> ListarEntregasAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Entregas
            .AsNoTracking()
            .OrderBy(entrega => entrega.DataPrevista)
            .Select(entrega => new EntregaResumo(
                entrega.Id,
                entrega.PedidoId,
                entrega.Pedido.Comprador.Nome,
                entrega.Status,
                entrega.DataPrevista,
                entrega.DataRealizada,
                entrega.Rota != null ? entrega.Rota.Motorista.Usuario.Nome : null,
                entrega.FotoUrl))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MotoristaResumo>> ListarMotoristasAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Motoristas
            .AsNoTracking()
            .Where(motorista => motorista.Ativo)
            .OrderBy(motorista => motorista.Usuario.Nome)
            .Select(motorista => new MotoristaResumo(
                motorista.Id,
                motorista.Usuario.Nome,
                motorista.CNH))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<VeiculoResumo>> ListarVeiculosAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Veiculos
            .AsNoTracking()
            .Where(veiculo => veiculo.Ativo)
            .OrderBy(veiculo => veiculo.Placa)
            .Select(veiculo => new VeiculoResumo(
                veiculo.Id,
                veiculo.Placa,
                veiculo.Tipo,
                veiculo.CapacidadeKg))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PedidoSemEntregaResumo>> ListarPedidosSemEntregaAsync(
        CancellationToken cancellationToken = default) =>
        await contexto.Pedidos
            .AsNoTracking()
            .Where(pedido => pedido.Entrega == null
                && pedido.Status != StatusPedido.Cancelado
                && pedido.Status != StatusPedido.Rascunho)
            .OrderBy(pedido => pedido.Id)
            .Select(pedido => new PedidoSemEntregaResumo(
                pedido.Id,
                pedido.Comprador.Nome,
                pedido.Status,
                pedido.ValorTotal,
                pedido.DataPedido))
            .ToListAsync(cancellationToken);
}
