using CloudNet.Domain.Identity;
using CloudNet.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudNet.Infrastructure.Persistence.Configurations.Identity;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(x => new { x.UserId, x.TokenHash })
            .IsUnique();

        builder.Property(x => x.IsRevoked)
            .HasDefaultValue(false);
    }
}
