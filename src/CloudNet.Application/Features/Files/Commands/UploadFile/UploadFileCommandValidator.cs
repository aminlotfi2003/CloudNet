using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.UploadFile;

public sealed class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.FolderId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SizeBytes).GreaterThan(0);
        RuleFor(x => x.Content).NotNull();
    }
}
