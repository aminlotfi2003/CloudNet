using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
