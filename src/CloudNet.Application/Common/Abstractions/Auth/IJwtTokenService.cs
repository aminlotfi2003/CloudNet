using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using System.Security.Claims;

namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IJwtTokenService
{
    AccessTokenResult GenerateAccessToken(
        ApplicationUser user,
        IEnumerable<string> roles,
        IEnumerable<Claim>? additionalClaims = null);
}
