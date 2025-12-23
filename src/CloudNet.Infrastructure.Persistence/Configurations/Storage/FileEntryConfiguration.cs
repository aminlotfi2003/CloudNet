using CloudNet.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudNet.Infrastructure.Persistence.Configurations.Storage;

public class FileEntryConfiguration : IEntityTypeConfiguration<FileEntry>
{
    public void Configure(EntityTypeBuilder<FileEntry> builder)
    {
        builder.ToTable("Files");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasIndex(x => new { x.OwnerId, x.FolderId });

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
    }
}
