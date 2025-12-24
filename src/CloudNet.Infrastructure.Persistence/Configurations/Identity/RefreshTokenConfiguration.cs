using CloudNet.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CloudNet.Infrastructure.Persistence.Context.CloudNetDbContext;

namespace CloudNet.Infrastructure.Persistence.Configurations.Identity;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", CloudNetDbSchema.Identity);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.FamilyId)
            .IsRequired();

        builder.Property(x => x.Device)
            .HasMaxLength(256);

        builder.Property(x => x.IsRevoked)
            .HasDefaultValue(false);

        builder.Property(x => x.RevokedAt);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId);

        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => x.IsRevoked);
        builder.HasIndex(x => x.FamilyId);
        builder.HasIndex(x => x.ReplacedByTokenId);

        builder.HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();
    }
}
