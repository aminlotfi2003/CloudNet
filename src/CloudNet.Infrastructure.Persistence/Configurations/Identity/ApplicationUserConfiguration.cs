using CloudNet.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CloudNet.Infrastructure.Persistence.Context.CloudNetDbContext;

namespace CloudNet.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users", CloudNetDbSchema.Identity);

        builder.Property(u => u.TenantId);

        builder.Property(u => u.PasswordLastChangedAt);

        builder.Property(u => u.MustChangePasswordOnNextLogin)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(x => x.RefreshTokens)
            .WithOne(t => t.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PasswordHistories)
            .WithOne(h => h.User)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.LoginHistories)
            .WithOne(h => h.User)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.NormalizedUserName).IsUnique();

        builder.HasIndex(u => u.NormalizedEmail).IsUnique();
    }
}
