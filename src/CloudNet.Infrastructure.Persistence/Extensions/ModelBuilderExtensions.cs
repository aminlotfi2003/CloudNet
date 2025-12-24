using CloudNet.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CloudNet.Infrastructure.Persistence.Context.CloudNetDbContext;

namespace CloudNet.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        var softDeleteTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType));

        foreach (var type in softDeleteTypes)
        {
            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(SetSoftDeleteFilter),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(type.ClrType);

            method.Invoke(null, [modelBuilder]);
        }
    }

    public static void MapIdentityTables(this ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", CloudNetDbSchema.Identity);
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", CloudNetDbSchema.Identity);
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", CloudNetDbSchema.Identity);
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", CloudNetDbSchema.Identity);
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", CloudNetDbSchema.Identity);
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
        where TEntity : class, ISoftDelete
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
