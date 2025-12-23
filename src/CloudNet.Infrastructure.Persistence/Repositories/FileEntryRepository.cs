using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Models;
using CloudNet.Domain.Storage;
using CloudNet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Repositories;

public sealed class FileEntryRepository : IFileEntryRepository
{
    private readonly CloudNetDbContext _db;

    public FileEntryRepository(CloudNetDbContext db)
    {
        _db = db;
    }

    public Task<FileEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<FileEntry?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Files
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<IReadOnlyList<FileEntry>> ListByFolderAsync(
        Guid ownerId,
        Guid folderId,
        CancellationToken cancellationToken = default)
        => _db.Files
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId && x.FolderId == folderId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<FileEntry>)t.Result, cancellationToken);

    public Task<IReadOnlyList<FileEntry>> ListDeletedAsync(Guid ownerId, CancellationToken cancellationToken = default)
        => _db.Files
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId && x.IsDeleted)
            .OrderByDescending(x => x.DeletedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<FileEntry>)t.Result, cancellationToken);

    public async Task<PagedResult<FileEntry>> SearchAsync(
        Guid ownerId,
        string? query,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        page.Validate();

        var q = _db.Files
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();
            q = q.Where(x =>
                x.FileName.Contains(query) ||
                (x.Description != null && x.Description.Contains(query)));
        }

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip(page.Skip)
            .Take(page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<FileEntry>
        {
            Items = items,
            TotalCount = total,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }

    public Task AddAsync(FileEntry file, CancellationToken cancellationToken = default)
        => _db.Files.AddAsync(file, cancellationToken).AsTask();

    public void Update(FileEntry file)
        => _db.Files.Update(file);

    public void Remove(FileEntry file)
        => _db.Files.Remove(file);
}
