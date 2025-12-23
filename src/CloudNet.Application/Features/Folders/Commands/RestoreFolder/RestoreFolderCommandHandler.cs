using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.RestoreFolder;

public sealed class RestoreFolderCommandHandler : IRequestHandler<RestoreFolderCommand>
{
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public RestoreFolderCommandHandler(IFolderRepository folders, IUnitOfWork uow, IDateTimeProvider clock)
    {
        _folders = folders;
        _uow = uow;
        _clock = clock;
    }

    public async Task Handle(RestoreFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await _folders.GetByIdIncludingDeletedAsync(request.FolderId, cancellationToken);
        if (folder is null || folder.OwnerId != request.OwnerId)
            throw new NotFoundException("Folder not found.");

        if (!folder.IsDeleted) return;

        var exists = await _folders.ExistsByNameAsync(folder.OwnerId, folder.ParentId, folder.Name, cancellationToken);
        if (exists)
            throw new ConflictException("Cannot restore folder because another active folder with the same name exists.");

        folder.IsDeleted = false;
        folder.DeletedAt = null;
        folder.ModifiedAt = _clock.UtcNow;

        _folders.Update(folder);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
