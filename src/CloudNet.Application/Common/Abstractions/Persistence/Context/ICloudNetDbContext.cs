using CloudNet.Domain.Identity;
using CloudNet.Domain.Storage;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Application.Common.Abstractions.Persistence.Context;

public interface ICloudNetDbContext
{
    DbSet<Folder> Folders { get; }
    DbSet<FileEntry> Files { get; }
    DbSet<StorageQuota> StorageQuotas { get; }
    DbSet<ShareLink> ShareLinks { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<LoginHistory> LoginHistories { get; }
    DbSet<PasswordHistory> PasswordHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
