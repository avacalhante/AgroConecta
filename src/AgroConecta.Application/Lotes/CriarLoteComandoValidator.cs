using FluentValidation;

namespace AgroConecta.Application.Lotes;

public class CriarLoteComandoValidator : AbstractValidator<CriarLoteComando>
{
    public CriarLoteComandoValidator()
    {
        RuleFor(comando => comando.ProdutoId)
            .GreaterThan(0).WithMessage("O produto é obrigatório.");

        RuleFor(comando => comando.CodigoRastreio)
            .NotEmpty().WithMessage("O código de rastreio é obrigatório.");

        RuleFor(comando => comando.DataColheita)
            .NotEmpty().WithMessage("A data de colheita é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("A data de colheita não pode ser futura.");

        RuleFor(comando => comando.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
    }
}
