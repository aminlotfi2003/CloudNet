using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.SoftDeleteFile;

public sealed class SoftDeleteFileCommandValidator : AbstractValidator<SoftDeleteFileCommand>
{
    public SoftDeleteFileCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
    }
}
