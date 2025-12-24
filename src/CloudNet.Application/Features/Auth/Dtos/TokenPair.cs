namespace CloudNet.Application.Features.Auth.Dtos;

public sealed record TokenPair(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    string RefreshTokenHash
);
