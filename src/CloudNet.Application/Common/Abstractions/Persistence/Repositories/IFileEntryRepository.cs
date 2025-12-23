using CloudNet.Application.Common.Models;
using CloudNet.Domain.Storage;

namespace CloudNet.Application.Common.Abstractions.Persistence.Repositories;

public interface IFileEntryRepository
{
    Task<FileEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FileEntry?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FileEntry>> ListByFolderAsync(
        Guid ownerId,
        Guid folderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FileEntry>> ListDeletedAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FileEntry>> SearchAsync(
        Guid ownerId,
        string? query,
        PageRequest page,
        CancellationToken cancellationToken = default);

    Task AddAsync(FileEntry file, CancellationToken cancellationToken = default);

    void Update(FileEntry file);

    void Remove(FileEntry file);
}
