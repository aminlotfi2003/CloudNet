using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.CreateFile;

public sealed class CreateFileEntryCommandValidator : AbstractValidator<CreateFileEntryCommand>
{
    public CreateFileEntryCommandValidator()
    {
        RuleFor(x => x.Dto.OwnerId).NotEmpty();
        RuleFor(x => x.Dto.FolderId).NotEmpty();

        RuleFor(x => x.Dto.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Dto.ContentType)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Dto.SizeBytes)
            .GreaterThan(0);

        RuleFor(x => x.Dto.StoragePath)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Dto.Description)
            .MaximumLength(2000);
    }
}
