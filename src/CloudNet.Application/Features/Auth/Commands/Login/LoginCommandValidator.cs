using FluentValidation;

namespace CloudNet.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
