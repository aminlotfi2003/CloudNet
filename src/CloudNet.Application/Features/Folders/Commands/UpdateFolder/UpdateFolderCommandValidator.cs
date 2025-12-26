using FluentValidation;

namespace CloudNet.Application.Features.Folders.Commands.UpdateFolder;

public sealed class UpdateFolderCommandValidator : AbstractValidator<UpdateFolderCommand>
{
    public UpdateFolderCommandValidator()
    {
        RuleFor(x => x.Dto.OwnerId).NotEmpty();
        RuleFor(x => x.Dto.FolderId).NotEmpty();

        RuleFor(x => x.Dto.Name)
            .NotEmpty()
            .MaximumLength(120);
    }
}
