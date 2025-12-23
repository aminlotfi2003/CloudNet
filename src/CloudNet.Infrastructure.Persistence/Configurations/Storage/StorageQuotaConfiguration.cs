using CloudNet.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudNet.Infrastructure.Persistence.Configurations.Storage;

public class StorageQuotaConfiguration : IEntityTypeConfiguration<StorageQuota>
{
    public void Configure(EntityTypeBuilder<StorageQuota> builder)
    {
        builder.ToTable("StorageQuotas");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.OwnerId)
            .IsUnique();

        builder.Property(x => x.QuotaBytes)
            .IsRequired();

        builder.Property(x => x.UsedBytes)
            .IsRequired();
    }
}
