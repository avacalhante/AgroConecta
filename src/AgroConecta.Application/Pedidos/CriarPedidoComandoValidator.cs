using FluentValidation;

namespace AgroConecta.Application.Pedidos;

public class CriarPedidoComandoValidator : AbstractValidator<CriarPedidoComando>
{
    public CriarPedidoComandoValidator()
    {
        RuleFor(comando => comando.CompradorId)
            .GreaterThan(0).WithMessage("O comprador é obrigatório.");

        RuleFor(comando => comando.EnderecoEntrega)
            .NotEmpty().WithMessage("O endereço de entrega é obrigatório.")
            .MaximumLength(300).WithMessage("O endereço deve ter até 300 caracteres.");

        RuleFor(comando => comando.Itens)
            .NotEmpty().WithMessage("O pedido deve ter pelo menos um item.");

        RuleForEach(comando => comando.Itens).ChildRules(item =>
        {
            item.RuleFor(detalhe => detalhe.ProdutoId)
                .GreaterThan(0).WithMessage("O produto é obrigatório.");

            item.RuleFor(detalhe => detalhe.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            item.RuleFor(detalhe => detalhe.PrecoUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("O preço unitário não pode ser negativo.");
        });
    }
}
