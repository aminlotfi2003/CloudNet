using FluentValidation;

namespace CloudNet.Application.Features.Files.Queries.ListDeleted;

public sealed class ListDeletedFilesQueryValidator : AbstractValidator<ListDeletedFilesQuery>
{
    public ListDeletedFilesQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
