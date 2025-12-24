using CloudNet.Domain.Common;

namespace CloudNet.Domain.Identity;

public class RefreshToken : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;

    public string TokenHash { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsRevoked { get; set; }

    public void Revoke() => IsRevoked = true;
    public bool IsExpired(DateTimeOffset utcNow) => ExpiresAt <= utcNow;
    public bool IsActive(DateTimeOffset utcNow) => !IsRevoked && !IsExpired(utcNow);
}
