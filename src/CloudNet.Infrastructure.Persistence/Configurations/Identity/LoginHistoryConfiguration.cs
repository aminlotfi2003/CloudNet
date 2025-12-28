using CloudNet.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CloudNet.Infrastructure.Persistence.Context.CloudNetDbContext;

namespace CloudNet.Infrastructure.Persistence.Configurations.Identity;

public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("LoginHistories", CloudNetDbSchema.Identity);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(64);

        builder.Property(x => x.UserAgent)
            .HasColumnName("Host")
            .HasMaxLength(255);

        builder.HasOne(x => x.User)
            .WithMany(u => u.LoginHistories)
            .HasForeignKey(x => x.UserId);

        builder.HasIndex(x => new { x.UserId, x.OccurredAt });
    }
}
