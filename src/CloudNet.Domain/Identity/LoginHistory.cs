using CloudNet.Domain.Common;

namespace CloudNet.Domain.Identity;

public class LoginHistory : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public string? IpAddress { get; set; }
    public string? Host { get; set; }
    public bool Success { get; set; }
    public int FailureCountBeforeSuccess { get; set; }
}
