using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Domain.Identity;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public class LoginHistoryRepository : ILoginHistoryRepository
{
    private readonly CloudNetDbContext _db;

    public LoginHistoryRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<LoginHistory>> GetRecentAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken = default)
    {
        count = count <= 0 ? 10 : count;

        return await _db.LoginHistories
            .Where(history => history.UserId == userId)
            .OrderByDescending(history => history.OccurredAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LoginHistory history, CancellationToken cancellationToken = default)
    {
        await _db.LoginHistories.AddAsync(history, cancellationToken);
    }
}
