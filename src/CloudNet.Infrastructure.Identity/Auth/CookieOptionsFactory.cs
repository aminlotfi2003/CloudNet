using Microsoft.AspNetCore.Http;

namespace CloudNet.Infrastructure.Identity.Auth;

public static class CookieOptionsFactory
{
    public static CookieOptions AccessToken(int minutes)
        => new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(minutes)
        };

    public static CookieOptions RefreshToken(int days)
        => new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(days)
        };
}
