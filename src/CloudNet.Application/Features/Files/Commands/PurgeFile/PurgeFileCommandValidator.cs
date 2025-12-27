using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.PurgeFile;

public sealed class PurgeFileCommandValidator : AbstractValidator<PurgeFileCommand>
{
    public PurgeFileCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
    }
}
