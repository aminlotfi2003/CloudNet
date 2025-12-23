using CloudNet.Domain.Common;

namespace CloudNet.Domain.Storage;

public class Folder : EntityBase<Guid>, IAuditableEntity, ISoftDelete
{
    public Guid OwnerId { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
