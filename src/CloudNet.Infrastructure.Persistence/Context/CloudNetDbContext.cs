using CloudNet.Domain.Identity;
using CloudNet.Domain.Storage;
using CloudNet.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudNet.Infrastructure.Persistence.Context;

public class CloudNetDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public CloudNetDbContext(DbContextOptions<CloudNetDbContext> options)
        : base(options)
    {
    }

    // Storage
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<FileEntry> Files => Set<FileEntry>();
    public DbSet<StorageQuota> StorageQuotas => Set<StorageQuota>();
    public DbSet<ShareLink> ShareLinks => Set<ShareLink>();

    // Identity
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(CloudNetDbContext).Assembly);

        builder.ApplySoftDeleteQueryFilter();

        builder.MapIdentityTables();
    }
}
