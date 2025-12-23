using FluentValidation;

namespace CloudNet.Application.Features.Files.Queries.ListByFolder;

public sealed class ListFilesByFolderQueryValidator : AbstractValidator<ListFilesByFolderQuery>
{
    public ListFilesByFolderQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FolderId).NotEmpty();
    }
}
