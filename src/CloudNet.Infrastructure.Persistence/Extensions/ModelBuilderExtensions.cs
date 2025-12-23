using CloudNet.Domain.Common;
using CloudNet.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            method.Invoke(null, new object[] { modelBuilder });
        }
    }

    public static void MapIdentityTables(this ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
        where TEntity : class, ISoftDelete
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
