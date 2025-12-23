using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.RestoreFile;

public sealed class RestoreFileCommandHandler : IRequestHandler<RestoreFileCommand>
{
    private readonly IFileEntryRepository _files;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public RestoreFileCommandHandler(IFileEntryRepository files, IUnitOfWork uow, IDateTimeProvider clock)
    {
        _files = files;
        _uow = uow;
        _clock = clock;
    }

    public async Task Handle(RestoreFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _files.GetByIdIncludingDeletedAsync(request.FileId, cancellationToken);
        if (file is null || file.OwnerId != request.OwnerId)
            throw new InvalidOperationException("File not found.");

        if (!file.IsDeleted) return;

        file.IsDeleted = false;
        file.DeletedAt = null;
        file.ModifiedAt = _clock.UtcNow;

        _files.Update(file);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
