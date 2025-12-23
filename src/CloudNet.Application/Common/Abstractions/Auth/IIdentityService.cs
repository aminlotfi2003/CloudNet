using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IIdentityService
{
    Task<(bool Succeeded, string[] Errors)> CreateUserAsync(
        string userName,
        string email,
        string password,
        CancellationToken ct);

    Task<ApplicationUser?> FindByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken ct);

    Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken ct);

    Task<bool> CheckPasswordAsync(ApplicationUser user, string password, CancellationToken ct);

    Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user, CancellationToken ct);
}
