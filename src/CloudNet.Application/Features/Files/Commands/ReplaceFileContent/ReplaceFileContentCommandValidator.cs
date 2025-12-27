using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.ReplaceFileContent;

public sealed class ReplaceFileContentCommandValidator : AbstractValidator<ReplaceFileContentCommand>
{
    public ReplaceFileContentCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SizeBytes).GreaterThan(0);
        RuleFor(x => x.Content).NotNull();
    }
}
