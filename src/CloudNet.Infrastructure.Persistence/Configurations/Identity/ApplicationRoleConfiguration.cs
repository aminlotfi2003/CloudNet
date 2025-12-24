using CloudNet.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CloudNet.Infrastructure.Persistence.Context.CloudNetDbContext;

namespace CloudNet.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles", CloudNetDbSchema.Identity);

        builder.Property(x => x.Description)
            .HasMaxLength(256);

        builder.HasIndex(r => r.NormalizedName).IsUnique();
    }
}
