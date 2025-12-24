using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using System.Security.Claims;

namespace CloudNet.Application.Common.Abstractions.Auth;

public interface ITokenService
{
    TokenPair GenerateTokenPair(
        ApplicationUser user,
        IEnumerable<string> roles,
        IEnumerable<Claim>? additionalClaims = null);

    string ComputeHash(string value);
}
