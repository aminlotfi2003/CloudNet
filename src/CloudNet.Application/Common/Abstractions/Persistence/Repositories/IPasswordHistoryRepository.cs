using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IPasswordHistoryRepository
{
    Task<IReadOnlyList<PasswordHistory>> GetRecentAsync(Guid userId, int count, CancellationToken cancellationToken = default);
    Task AddAsync(PasswordHistory history, CancellationToken cancellationToken = default);
    Task PruneExcessAsync(Guid userId, int maxEntries, CancellationToken cancellationToken = default);
}
