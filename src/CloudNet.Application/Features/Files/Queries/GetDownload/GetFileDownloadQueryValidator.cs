using FluentValidation;

namespace CloudNet.Application.Features.Files.Queries.GetDownload;

public sealed class GetFileDownloadQueryValidator : AbstractValidator<GetFileDownloadQuery>
{
    public GetFileDownloadQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
    }
}
