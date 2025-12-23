using FluentValidation;

namespace CloudNet.Application.Features.Folders.Queries.ListChildren;

public sealed class ListFolderChildrenQueryValidator : AbstractValidator<ListFolderChildrenQuery>
{
    public ListFolderChildrenQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
