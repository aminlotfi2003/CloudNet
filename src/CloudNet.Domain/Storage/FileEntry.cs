using CloudNet.Domain.Common;

namespace CloudNet.Domain.Storage;

public class FileEntry : EntityBase<Guid>, IAuditableEntity, ISoftDelete
{
    public Guid OwnerId { get; set; }
    public Guid FolderId { get; set; }

    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long SizeBytes { get; set; }

    public string StoragePath { get; set; } = default!;

    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
