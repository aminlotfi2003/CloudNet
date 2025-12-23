using CloudNet.Domain.Storage;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IFolderRepository
{
    Task<Folder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Folder?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Folder>> ListChildrenAsync(
        Guid ownerId,
        Guid? parentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Folder>> ListDeletedAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        Guid ownerId,
        Guid? parentId,
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(Folder folder, CancellationToken cancellationToken = default);

    void Update(Folder folder);
}
