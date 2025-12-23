using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.SoftDeleteFolder;

public sealed class SoftDeleteFolderCommandHandler : IRequestHandler<SoftDeleteFolderCommand>
{
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public SoftDeleteFolderCommandHandler(IFolderRepository folders, IUnitOfWork uow, IDateTimeProvider clock)
    {
        _folders = folders;
        _uow = uow;
        _clock = clock;
    }

    public async Task Handle(SoftDeleteFolderCommand request, CancellationToken cancellationToken)
    {
        var folder = await _folders.GetByIdAsync(request.FolderId, cancellationToken);
        if (folder is null || folder.OwnerId != request.OwnerId)
            throw new NotFoundException("Folder not found.");

        folder.IsDeleted = true;
        folder.DeletedAt = _clock.UtcNow;
        folder.ModifiedAt = _clock.UtcNow;

        _folders.Update(folder);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
