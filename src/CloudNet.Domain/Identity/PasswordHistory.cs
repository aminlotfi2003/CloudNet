using CloudNet.Domain.Common;

namespace CloudNet.Domain.Identity;

public class PasswordHistory : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string PasswordHash { get; set; } = default!;
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;

    public static PasswordHistory Create(Guid userId, string passwordHash, DateTimeOffset changedAt)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PasswordHash = passwordHash,
            ChangedAt = changedAt
        };
}
