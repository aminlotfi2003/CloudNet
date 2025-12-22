namespace CloudNet.WebFramework.Entities;

public class File
{
    public Guid Id { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastModifiedAt { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;

    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string FileType { get; set; } = default!;
    public long FileSize { get; set; }
    public string? Description { get; set; }
    
    public Guid ParentFolderId { get; set; }
    public Folder Folder { get; set; } = default!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
