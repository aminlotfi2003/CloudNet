using CloudNet.Domain.Storage;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IStorageQuotaRepository
{
    Task<StorageQuota?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task AddAsync(StorageQuota quota, CancellationToken cancellationToken = default);

    void Update(StorageQuota quota);
}
