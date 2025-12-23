using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Infrastructure.Persistence.Context;

namespace CloudNet.Infrastructure.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CloudNetDbContext _db;

    public UnitOfWork(CloudNetDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
