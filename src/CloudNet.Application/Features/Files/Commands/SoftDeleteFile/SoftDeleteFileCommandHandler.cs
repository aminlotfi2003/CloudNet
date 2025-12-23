using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.SoftDeleteFile;

public sealed class SoftDeleteFileCommandHandler : IRequestHandler<SoftDeleteFileCommand>
{
    private readonly IFileEntryRepository _files;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public SoftDeleteFileCommandHandler(IFileEntryRepository files, IUnitOfWork uow, IDateTimeProvider clock)
    {
        _files = files;
        _uow = uow;
        _clock = clock;
    }

    public async Task Handle(SoftDeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _files.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null || file.OwnerId != request.OwnerId)
            throw new InvalidOperationException("File not found.");

        file.IsDeleted = true;
        file.DeletedAt = _clock.UtcNow;
        file.ModifiedAt = _clock.UtcNow;

        _files.Update(file);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
