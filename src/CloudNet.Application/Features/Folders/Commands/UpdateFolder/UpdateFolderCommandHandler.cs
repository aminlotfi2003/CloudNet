using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.UpdateFolder;

public sealed class UpdateFolderCommandHandler : IRequestHandler<UpdateFolderCommand, FolderDto>
{
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateFolderCommandHandler(
        IFolderRepository folders,
        IUnitOfWork uow,
        IDateTimeProvider clock,
        AutoMapper.IMapper mapper)
    {
        _folders = folders;
        _uow = uow;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<FolderDto> Handle(UpdateFolderCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        var newName = dto.Name.Trim();

        var folder = await _folders.GetByIdAsync(dto.FolderId, ct);
        if (folder is null || folder.OwnerId != dto.OwnerId)
            throw new NotFoundException("Folder not found.");

        if (folder.IsDeleted)
            throw new ConflictException("Cannot update a deleted folder. Restore it first.");

        if (string.Equals(folder.Name, newName, StringComparison.Ordinal))
            return _mapper.Map<FolderDto>(folder);

        var exists = await _folders.ExistsByNameAsync(folder.OwnerId, folder.ParentId, newName, ct);
        if (exists)
            throw new ConflictException("A folder with the same name already exists in this location.");

        folder.Name = newName;
        folder.ModifiedAt = _clock.UtcNow;

        _folders.Update(folder);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<FolderDto>(folder);
    }
}
