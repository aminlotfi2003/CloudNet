using Microsoft.AspNetCore.Identity;
using CloudNet.Domain.Common;

namespace CloudNet.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    public Guid? TenantId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; set; }

    public DateTimeOffset? PasswordLastChangedAt { get; set; }
    public bool MustChangePasswordOnNextLogin { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<LoginHistory> LoginHistories { get; set; } = new HashSet<LoginHistory>();
    public ICollection<PasswordHistory> PasswordHistories { get; set; } = new HashSet<PasswordHistory>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}
