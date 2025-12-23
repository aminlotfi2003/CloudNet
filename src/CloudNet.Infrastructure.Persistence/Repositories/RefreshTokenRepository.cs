using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Domain.Identity;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CloudNetDbContext _db;

    public RefreshTokenRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<RefreshToken?> GetByUserAndTokenHashAsync(
        Guid userId,
        string tokenHash,
        CancellationToken cancellationToken = default)
        => _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TokenHash == tokenHash, cancellationToken);

    public Task<IReadOnlyList<RefreshToken>> ListValidByUserAsync(
        Guid userId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
        => _db.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiresAt > nowUtc)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<RefreshToken>)t.Result, cancellationToken);

    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        => _db.RefreshTokens.AddAsync(token, cancellationToken).AsTask();

    public void Update(RefreshToken token)
        => _db.RefreshTokens.Update(token);
}
