using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Domain.Storage;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public sealed class ShareLinkRepository : IShareLinkRepository
{
    private readonly CloudNetDbContext _db;

    public ShareLinkRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public Task<ShareLink?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.ShareLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<IReadOnlyList<ShareLink>> ListByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
        => _db.ShareLinks
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<ShareLink>)t.Result, cancellationToken);

    public Task<IReadOnlyList<ShareLink>> ListByFileAsync(Guid ownerId, Guid fileId, CancellationToken cancellationToken = default)
        => _db.ShareLinks
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId && x.FileId == fileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<ShareLink>)t.Result, cancellationToken);

    public Task<ShareLink?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => _db.ShareLinks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public Task AddAsync(ShareLink link, CancellationToken cancellationToken = default)
        => _db.ShareLinks.AddAsync(link, cancellationToken).AsTask();

    public void Update(ShareLink link)
        => _db.ShareLinks.Update(link);
}
