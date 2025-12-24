using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IPasswordPolicyService
{
    Task EnsurePasswordCompliesAsync(ApplicationUser user, string newPassword, CancellationToken cancellationToken = default);
    Task RecordPasswordChangeAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken = default);
    bool IsPasswordExpired(ApplicationUser user, DateTimeOffset utcNow, int days = 90);
}
