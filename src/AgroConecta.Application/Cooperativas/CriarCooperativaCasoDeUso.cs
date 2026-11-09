using AgroConecta.Application.Common;
using AgroConecta.Domain.Abstractions;
using AgroConecta.Domain.Entities;
using FluentValidation;

namespace AgroConecta.Application.Cooperativas;

public record CriarCooperativaComando(string Nome, string Cidade, string UF, string Tipo);

public class CriarCooperativaCasoDeUso(
    ICooperativaRepositorio repositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IValidator<CriarCooperativaComando> validador)
    : ICasoDeUso<CriarCooperativaComando, int>
{
    public async Task<int> ExecutarAsync(
        CriarCooperativaComando entrada,
        CancellationToken cancellationToken = default)
    {
        await validador.ValidarAsync(entrada, cancellationToken);

        var cooperativa = new Cooperativa
        {
            Nome = entrada.Nome.Trim(),
            Cidade = entrada.Cidade.Trim(),
            UF = entrada.UF.Trim().ToUpperInvariant(),
            Tipo = entrada.Tipo.Trim(),
            Ativa = true,
        };

        await repositorio.AdicionarAsync(cooperativa, cancellationToken);
        await unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return cooperativa.Id;
    }
}

public class CriarCooperativaComandoValidator : AbstractValidator<CriarCooperativaComando>
{
    public CriarCooperativaComandoValidator()
    {
        RuleFor(comando => comando.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter até 150 caracteres.");

        RuleFor(comando => comando.Cidade)
            .NotEmpty().WithMessage("A cidade é obrigatória.")
            .MaximumLength(100).WithMessage("A cidade deve ter até 100 caracteres.");

        RuleFor(comando => comando.UF)
            .NotEmpty().WithMessage("A UF é obrigatória.")
            .Length(2).WithMessage("A UF deve ter duas letras.");

        RuleFor(comando => comando.Tipo)
            .NotEmpty().WithMessage("O tipo é obrigatório.")
            .MaximumLength(60).WithMessage("O tipo deve ter até 60 caracteres.");
    }
}
