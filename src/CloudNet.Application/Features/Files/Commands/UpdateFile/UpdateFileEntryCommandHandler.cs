using AutoMapper;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.UpdateFile;

public sealed class UpdateFileEntryCommandHandler : IRequestHandler<UpdateFileEntryCommand, FileEntryDto>
{
    private readonly IFileEntryRepository _files;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;
    private readonly IMapper _mapper;

    public UpdateFileEntryCommandHandler(
        IFileEntryRepository files,
        IUnitOfWork uow,
        IDateTimeProvider clock,
        IMapper mapper)
    {
        _files = files;
        _uow = uow;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<FileEntryDto> Handle(UpdateFileEntryCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var file = await _files.GetByIdAsync(dto.FileId, ct);
        if (file is null || file.OwnerId != dto.OwnerId)
            throw new NotFoundException("File not found.");

        if (file.IsDeleted)
            throw new ConflictException("Cannot update a deleted file. Restore it first.");

        var changed = false;

        if (dto.FileName is not null)
        {
            var newName = dto.FileName.Trim();
            if (!string.Equals(file.FileName, newName, StringComparison.Ordinal))
            {
                file.FileName = newName;
                changed = true;
            }
        }

        if (dto.Description is not null)
        {
            var newDesc = dto.Description.Trim();
            if (!string.Equals(file.Description ?? string.Empty, newDesc, StringComparison.Ordinal))
            {
                file.Description = string.IsNullOrWhiteSpace(newDesc) ? null : newDesc;
                changed = true;
            }
        }

        if (!changed)
            return _mapper.Map<FileEntryDto>(file);

        file.ModifiedAt = _clock.UtcNow;

        _files.Update(file);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<FileEntryDto>(file);
    }
}
