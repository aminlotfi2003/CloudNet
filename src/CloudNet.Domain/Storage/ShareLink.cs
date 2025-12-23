using CloudNet.Domain.Common;
using CloudNet.Domain.Enums;

namespace CloudNet.Domain.Storage;

public class ShareLink : EntityBase<Guid>
{
    public Guid OwnerId { get; set; }
    public Guid FileId { get; set; }

    public string TokenHash { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresAt { get; set; }

    public int? MaxDownloads { get; set; }
    public int DownloadsCount { get; set; }

    public bool IsRevoked { get; set; }

    public SharePermission Permission { get; set; } = SharePermission.ReadOnly;
}
