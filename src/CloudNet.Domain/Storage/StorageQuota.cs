using CloudNet.Domain.Common;

namespace CloudNet.Domain.Storage;

public class StorageQuota : EntityBase<Guid>
{
    public Guid OwnerId { get; set; }

    public long QuotaBytes { get; set; }
    public long UsedBytes { get; set; }

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
