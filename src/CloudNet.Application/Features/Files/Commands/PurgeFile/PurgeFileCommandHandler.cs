using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Domain.Storage;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.PurgeFile;

public sealed class PurgeFileCommandHandler : IRequestHandler<PurgeFileCommand>
{
    private readonly IFileEntryRepository _files;
    private readonly IStorageQuotaRepository _quotas;
    private readonly IFileStorage _storage;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;
    private readonly IStorageQuotaSettings _quotaSettings;

    public PurgeFileCommandHandler(
        IFileEntryRepository files,
        IStorageQuotaRepository quotas,
        IFileStorage storage,
        IUnitOfWork uow,
        IDateTimeProvider clock,
        IStorageQuotaSettings quotaSettings)
    {
        _files = files;
        _quotas = quotas;
        _storage = storage;
        _uow = uow;
        _clock = clock;
        _quotaSettings = quotaSettings;
    }

    public async Task Handle(PurgeFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _files.GetByIdIncludingDeletedAsync(request.FileId, cancellationToken);
        if (file is null || file.OwnerId != request.OwnerId)
            throw new NotFoundException("File not found.");

        if (!file.IsDeleted)
            throw new BusinessRuleViolationException("File must be deleted before purge.");

        var (quota, isNewQuota) = await EnsureQuotaAsync(request.OwnerId, cancellationToken);

        _files.Remove(file);

        quota.UsedBytes = Math.Max(0, quota.UsedBytes - file.SizeBytes);
        quota.UpdatedAt = _clock.UtcNow;
        if (!isNewQuota)
            _quotas.Update(quota);

        await _uow.SaveChangesAsync(cancellationToken);

        await _storage.DeleteAsync(file.StoragePath, cancellationToken);
    }

    private async Task<(StorageQuota quota, bool isNew)> EnsureQuotaAsync(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var quota = await _quotas.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (quota is not null)
            return (quota, false);

        if (_quotaSettings.DefaultQuotaBytes <= 0)
            throw new BusinessRuleViolationException("Storage quota default is not configured.");

        var usedBytes = await _files.GetTotalSizeByOwnerAsync(ownerId, cancellationToken);

        quota = new StorageQuota
        {
            OwnerId = ownerId,
            QuotaBytes = _quotaSettings.DefaultQuotaBytes,
            UsedBytes = usedBytes,
            UpdatedAt = _clock.UtcNow
        };

        await _quotas.AddAsync(quota, cancellationToken);

        return (quota, true);
    }
}
