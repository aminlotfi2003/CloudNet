using FluentValidation;

namespace CloudNet.Application.Features.Files.Queries.GetSharedDownload;

public sealed class GetSharedFileDownloadQueryValidator : AbstractValidator<GetSharedFileDownloadQuery>
{
    public GetSharedFileDownloadQueryValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
