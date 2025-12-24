using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface ILoginHistoryRepository
{
    Task<IReadOnlyList<LoginHistory>> GetRecentAsync(Guid userId, int count, CancellationToken cancellationToken = default);
    Task AddAsync(LoginHistory history, CancellationToken cancellationToken = default);
}
