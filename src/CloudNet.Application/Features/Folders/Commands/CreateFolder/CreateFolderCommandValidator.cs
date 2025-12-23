using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using FluentValidation;

namespace CloudNet.Application.Features.Folders.Commands.CreateFolder;

public sealed class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
{
    public CreateFolderCommandValidator(IFolderRepository folderRepository)
    {
        RuleFor(x => x.Dto.OwnerId)
            .NotEmpty();

        RuleFor(x => x.Dto.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                return !await folderRepository.ExistsByNameAsync(
                    cmd.Dto.OwnerId, cmd.Dto.ParentId, cmd.Dto.Name.Trim(), ct);
            })
            .WithMessage("A folder with the same name already exists in this location.");
    }
}
