using FluentValidation;

namespace CloudNet.Application.Features.Folders.Commands.RestoreFolder;

public sealed class RestoreFolderCommandValidator : AbstractValidator<RestoreFolderCommand>
{
    public RestoreFolderCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FolderId).NotEmpty();
    }
}
