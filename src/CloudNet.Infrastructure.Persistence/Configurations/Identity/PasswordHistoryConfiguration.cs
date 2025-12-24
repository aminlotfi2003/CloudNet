using CloudNet.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CloudNet.Infrastructure.Persistence.Context.CloudNetDbContext;

namespace CloudNet.Infrastructure.Persistence.Configurations.Identity;

public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("PasswordHistories", CloudNetDbSchema.Identity);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PasswordHash).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(u => u.PasswordHistories)
            .HasForeignKey(x => x.UserId);

        builder.HasIndex(x => new { x.UserId, x.ChangedAt });
    }
}
