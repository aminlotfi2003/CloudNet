using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Infrastructure.Identity.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CloudNet.Infrastructure.Identity.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly JwtOptions _options;

    public RefreshTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public int RefreshTokenDays => _options.RefreshTokenDays;

    public string GenerateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string rawToken)
    {
        var pepperBytes = Encoding.UTF8.GetBytes(_options.RefreshTokenPepper);

        using var hmac = new HMACSHA256(pepperBytes);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawToken));

        return Convert.ToHexString(hashBytes);
    }
}
