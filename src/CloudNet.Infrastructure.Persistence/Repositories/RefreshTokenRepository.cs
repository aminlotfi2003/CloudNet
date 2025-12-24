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

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await _db.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _db.RefreshTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }

    public async Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }
}
