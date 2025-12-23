using FluentValidation;

namespace CloudNet.Application.Features.Folders.Commands.SoftDeleteFolder;

public sealed class SoftDeleteFolderCommandValidator : AbstractValidator<SoftDeleteFolderCommand>
{
    public SoftDeleteFolderCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FolderId).NotEmpty();
    }
}
