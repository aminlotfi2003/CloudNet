using FluentValidation;

namespace CloudNet.Application.Features.Folders.Queries.ListDeleted;

public sealed class ListDeletedFoldersQueryValidator : AbstractValidator<ListDeletedFoldersQuery>
{
    public ListDeletedFoldersQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
