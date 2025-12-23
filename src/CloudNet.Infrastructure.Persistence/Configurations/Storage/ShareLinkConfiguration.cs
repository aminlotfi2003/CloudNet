using CloudNet.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudNet.Infrastructure.Persistence.Configurations.Storage;

public class ShareLinkConfiguration : IEntityTypeConfiguration<ShareLink>
{
    public void Configure(EntityTypeBuilder<ShareLink> builder)
    {
        builder.ToTable("ShareLinks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.IsRevoked)
            .HasDefaultValue(false);
    }
}
