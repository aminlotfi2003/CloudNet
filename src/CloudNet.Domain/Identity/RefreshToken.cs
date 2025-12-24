using CloudNet.Domain.Common;

namespace CloudNet.Domain.Identity;

public class RefreshToken : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;

    public Guid FamilyId { get; set; }
    public string TokenHash { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsRevoked { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public string? Device { get; set; }

    public byte[]? RowVersion { get; set; }

    public void Revoke(DateTimeOffset revokedAt, Guid? replacedByTokenId = null)
    {
        IsRevoked = true;
        RevokedAt = revokedAt;
        ReplacedByTokenId = replacedByTokenId;
    }
    public bool IsExpired(DateTimeOffset utcNow) => ExpiresAt <= utcNow;
    public bool IsActive(DateTimeOffset utcNow) => !IsRevoked && !IsExpired(utcNow);
}
