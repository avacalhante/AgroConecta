using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using AgroConecta.Domain.Enums;
using AgroConecta.Domain.Exceptions;
using FluentValidation;

namespace AgroConecta.Application.Rotas;

public class CriarRotaCasoDeUso(
    IRotaRepositorio rotas,
    IPedidoRepositorio pedidos,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarRotaComando> validador)
    : ICasoDeUso<CriarRotaComando, RotaDto>
{
    public async Task<RotaDto> ExecutarAsync(
        CriarRotaComando entrada,
        CancellationToken cancellationToken = default)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        if (!await rotas.MotoristaAtivoExisteAsync(entrada.MotoristaId, cancellationToken))
        {
            throw new RecursoNaoEncontradoException("Motorista ativo não encontrado.");
        }

        if (!await rotas.VeiculoAtivoExisteAsync(entrada.VeiculoId, cancellationToken))
        {
            throw new RecursoNaoEncontradoException("Veículo ativo não encontrado.");
        }

        var rota = new Rota
        {
            Data = entrada.Data,
            MotoristaId = entrada.MotoristaId,
            VeiculoId = entrada.VeiculoId,
            Status = StatusRota.Planejada,
        };

        foreach (var pedidoId in entrada.PedidoIds.Distinct())
        {
            var pedido = await pedidos.ObterPorIdAsync(pedidoId, cancellationToken)
                ?? throw new RecursoNaoEncontradoException($"Pedido {pedidoId} não encontrado.");

            if (await rotas.PedidoPossuiEntregaAsync(pedidoId, cancellationToken))
            {
                throw new RegraNegocioException(
                    $"O pedido {pedidoId} já está vinculado a uma entrega.");
            }

            rota.Entregas.Add(new Entrega
            {
                Pedido = pedido,
                PedidoId = pedido.Id,
                DataPrevista = entrada.Data,
            });
        }

        await rotas.AdicionarAsync(rota, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return MapeadorRota.ParaDto(rota);
    }
}

public class ListarRotasCasoDeUso(IRotaRepositorio rotas)
    : ICasoDeUso<bool, IReadOnlyCollection<RotaDto>>
{
    public async Task<IReadOnlyCollection<RotaDto>> ExecutarAsync(
        bool entrada,
        CancellationToken cancellationToken = default)
    {
        var resultado = await rotas.ListarAsync(cancellationToken);
        return resultado.Select(MapeadorRota.ParaDto).ToArray();
    }
}

public class CriarRotaComandoValidator : AbstractValidator<CriarRotaComando>
{
    public CriarRotaComandoValidator()
    {
        RuleFor(comando => comando.Data)
            .NotEmpty().WithMessage("A data da rota é obrigatória.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("A data da rota não pode estar no passado.");

        RuleFor(comando => comando.MotoristaId)
            .GreaterThan(0).WithMessage("O motorista é obrigatório.");

        RuleFor(comando => comando.VeiculoId)
            .GreaterThan(0).WithMessage("O veículo é obrigatório.");

        RuleFor(comando => comando.PedidoIds)
            .NotEmpty().WithMessage("Selecione ao menos um pedido para a rota.");
    }
}
