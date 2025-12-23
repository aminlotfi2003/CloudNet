using CloudNet.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudNet.Infrastructure.Persistence.Configurations.Storage;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.ToTable("Folders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(x => new { x.OwnerId, x.ParentId, x.Name })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
    }
}
