using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Auth;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using CloudNet.Infrastructure.Identity.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CloudNet.Infrastructure.Identity.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly IDateTimeProvider _clock;

    public JwtTokenService(IOptions<JwtOptions> options, IDateTimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;
    }

    public AccessTokenResult GenerateAccessToken(
        ApplicationUser user,
        IEnumerable<string> roles,
        IEnumerable<Claim>? additionalClaims = null)
    {
        var now = _clock.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtClaimTypes.OwnerId, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.UserName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        if (additionalClaims is not null)
            claims.AddRange(additionalClaims);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: accessTokenExpiresAt.UtcDateTime,
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(jwt);

        return new AccessTokenResult(accessToken, accessTokenExpiresAt);
    }
}
