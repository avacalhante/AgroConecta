using FluentValidation;

namespace AgroConecta.Application.Produtos;

public class CriarProdutoComandoValidator : AbstractValidator<CriarProdutoComando>
{
    public CriarProdutoComandoValidator()
    {
        RuleFor(comando => comando.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(120).WithMessage("O nome deve ter até 120 caracteres.");

        RuleFor(comando => comando.Categoria)
            .NotEmpty().WithMessage("A categoria é obrigatória.")
            .MaximumLength(80).WithMessage("A categoria deve ter até 80 caracteres.");

        RuleFor(comando => comando.Unidade)
            .NotEmpty().WithMessage("A unidade é obrigatória.")
            .MaximumLength(20).WithMessage("A unidade deve ter até 20 caracteres.");

        RuleFor(comando => comando.PrecoBase)
            .GreaterThanOrEqualTo(0).WithMessage("O preço-base não pode ser negativo.");
    }
}
