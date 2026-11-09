using FluentValidation;

namespace AgroConecta.Application.Auth;

public class LoginComandoValidator : AbstractValidator<LoginComando>
{
    public LoginComandoValidator()
    {
        RuleFor(comando => comando.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Informe um e-mail válido.")
            .MaximumLength(200).WithMessage("O e-mail deve ter até 200 caracteres.");

        RuleFor(comando => comando.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.");
    }
}
