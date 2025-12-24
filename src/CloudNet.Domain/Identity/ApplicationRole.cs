using CloudNet.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace CloudNet.Domain.Identity;

public class ApplicationRole : IdentityRole<Guid>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; set; }
    
    public string? Description { get; set; }
}
