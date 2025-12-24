namespace CloudNet.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SigningKey { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 30;
    public bool RevokeAllTokensOnReuse { get; init; } = true;
}
