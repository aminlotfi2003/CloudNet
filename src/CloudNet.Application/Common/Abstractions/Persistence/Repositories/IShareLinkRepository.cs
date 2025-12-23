using CloudNet.Domain.Storage;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IShareLinkRepository
{
    Task<ShareLink?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShareLink>> ListByOwnerAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShareLink>> ListByFileAsync(
        Guid ownerId,
        Guid fileId,
        CancellationToken cancellationToken = default);

    Task<ShareLink?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task AddAsync(ShareLink link, CancellationToken cancellationToken = default);

    void Update(ShareLink link);
}
