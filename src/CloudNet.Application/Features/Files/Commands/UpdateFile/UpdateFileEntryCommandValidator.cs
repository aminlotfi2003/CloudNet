using FluentValidation;

namespace CloudNet.Application.Features.Files.Commands.UpdateFile;

public sealed class UpdateFileEntryCommandValidator : AbstractValidator<UpdateFileEntryCommand>
{
    public UpdateFileEntryCommandValidator()
    {
        RuleFor(x => x.Dto.OwnerId).NotEmpty();
        RuleFor(x => x.Dto.FileId).NotEmpty();

        RuleFor(x => x.Dto.FileName)
            .MaximumLength(255);

        RuleFor(x => x.Dto.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Dto)
            .Must(dto => dto.FileName is not null || dto.Description is not null)
            .WithMessage("At least one field must be provided to update.");
    }
}
