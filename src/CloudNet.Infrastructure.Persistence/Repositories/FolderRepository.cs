using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Domain.Storage;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public sealed class FolderRepository : IFolderRepository
{
    private readonly CloudNetDbContext _db;

    public FolderRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public Task<Folder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Folders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Folder?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Folders
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<IReadOnlyList<Folder>> ListChildrenAsync(
        Guid ownerId,
        Guid? parentId,
        CancellationToken cancellationToken = default)
        => _db.Folders
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId && x.ParentId == parentId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<Folder>)t.Result, cancellationToken);

    public Task<IReadOnlyList<Folder>> ListDeletedAsync(Guid ownerId, CancellationToken cancellationToken = default)
        => _db.Folders
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId && x.IsDeleted)
            .OrderByDescending(x => x.DeletedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<Folder>)t.Result, cancellationToken);

    public Task<bool> ExistsByNameAsync(
        Guid ownerId,
        Guid? parentId,
        string name,
        CancellationToken cancellationToken = default)
        => _db.Folders
            .AsNoTracking()
            .AnyAsync(x =>
                x.OwnerId == ownerId &&
                x.ParentId == parentId &&
                x.Name == name, cancellationToken);

    public Task AddAsync(Folder folder, CancellationToken cancellationToken = default)
        => _db.Folders.AddAsync(folder, cancellationToken).AsTask();

    public void Update(Folder folder)
        => _db.Folders.Update(folder);
}
