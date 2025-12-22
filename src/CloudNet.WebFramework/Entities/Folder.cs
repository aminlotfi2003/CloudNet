namespace CloudNet.WebFramework.Entities;

public class Folder
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastModifiedAt { get; set; }

    public string FolderName { get; set; } = default!;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;

    public Guid ParentFolderId { get; set; }
    public Folder ParentFolder { get; set; } = default!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<Folder> SubFolders { get; set; } = new HashSet<Folder>();
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
