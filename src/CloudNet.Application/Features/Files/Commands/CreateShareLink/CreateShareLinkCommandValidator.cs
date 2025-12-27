using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.CreateShareLink;

public sealed class CreateShareLinkCommandValidator : AbstractValidator<CreateShareLinkCommand>
{
    public CreateShareLinkCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.ExpiresAt)
            .Must(expiry => expiry is null || expiry > DateTimeOffset.UtcNow)
            .WithMessage("Expiry must be in the future.");
        RuleFor(x => x.MaxDownloads)
            .Must(max => max is null || max > 0)
            .WithMessage("Max downloads must be greater than zero.");
    }
}
