using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Domain.Identity;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public class PasswordHistoryRepository : IPasswordHistoryRepository
{
    private readonly CloudNetDbContext _db;

    public PasswordHistoryRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PasswordHistory>> GetRecentAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _db.PasswordHistories
            .Where(history => history.UserId == userId)
            .OrderByDescending(history => history.ChangedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PasswordHistory history, CancellationToken cancellationToken = default)
    {
        await _db.PasswordHistories.AddAsync(history, cancellationToken);
    }

    public async Task PruneExcessAsync(Guid userId, int maxEntries, CancellationToken cancellationToken = default)
    {
        var toRemove = await _db.PasswordHistories
            .Where(history => history.UserId == userId)
            .OrderByDescending(history => history.ChangedAt)
            .Skip(maxEntries)
            .ToListAsync(cancellationToken);

        if (toRemove.Count != 0)
            _db.PasswordHistories.RemoveRange(toRemove);
    }
}
