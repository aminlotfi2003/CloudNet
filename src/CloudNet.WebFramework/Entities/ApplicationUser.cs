using Microsoft.AspNetCore.Identity;

namespace CloudNet.WebFramework.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastModifiedAt { get; set; }
    
    public long StorageLimit { get; set; }
    public long StorageUsed { get; set; }

    public ICollection<File> Files { get; set; } = new HashSet<File>();
    public ICollection<Folder> Folders { get; set; } = new HashSet<Folder>();
}
