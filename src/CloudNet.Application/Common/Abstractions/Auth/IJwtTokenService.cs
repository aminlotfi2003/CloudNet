using CloudNet.Domain.Identity;

namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IJwtTokenService
{
    string CreateAccessToken(ApplicationUser user, IReadOnlyList<string> roles);
    int AccessTokenMinutes { get; }
}
