using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Domain.Storage;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public sealed class StorageQuotaRepository : IStorageQuotaRepository
{
    private readonly CloudNetDbContext _db;

    public StorageQuotaRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public Task<StorageQuota?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
        => _db.StorageQuotas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OwnerId == ownerId, cancellationToken);

    public Task AddAsync(StorageQuota quota, CancellationToken cancellationToken = default)
        => _db.StorageQuotas.AddAsync(quota, cancellationToken).AsTask();

    public void Update(StorageQuota quota)
        => _db.StorageQuotas.Update(quota);
}
