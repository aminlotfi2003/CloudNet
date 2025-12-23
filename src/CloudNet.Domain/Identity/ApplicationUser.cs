using Microsoft.AspNetCore.Identity;
using CloudNet.Domain.Common;

namespace CloudNet.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; set; }

    public bool IsActive { get; set; } = true;
}
