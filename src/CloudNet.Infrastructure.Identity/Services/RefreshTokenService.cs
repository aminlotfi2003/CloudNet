using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Infrastructure.Identity.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CloudNet.Infrastructure.Identity.Services;

public sealed class RefreshTokenService : IRefreshTokenService, IRefreshTokenSettings
{
    private readonly JwtOptions _options;
    private readonly IDateTimeProvider _clock;

    public RefreshTokenService(IOptions<JwtOptions> options, IDateTimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;
    }

    public bool RevokeAllTokensOnReuse => _options.RevokeAllTokensOnReuse;

    public RefreshTokenResult GenerateRefreshToken(Guid? familyId = null)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = HashToken(token);
        var tokenId = Guid.NewGuid();
        var family = familyId ?? tokenId;
        var expiresAt = _clock.UtcNow.AddDays(_options.RefreshTokenDays);

        return new RefreshTokenResult(
            tokenId,
            family,
            token,
            tokenHash,
            expiresAt);
    }

    public string HashToken(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
