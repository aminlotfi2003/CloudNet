using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByUserAndTokenHashAsync(
        Guid userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RefreshToken>> ListValidByUserAsync(
        Guid userId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);

    void Update(RefreshToken token);
}
