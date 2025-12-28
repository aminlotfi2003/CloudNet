using FluentValidation;

namespace CloudNet.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty();

        RuleFor(x => x.ResetToken)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty();
    }
}
