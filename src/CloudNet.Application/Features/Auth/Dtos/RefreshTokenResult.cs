namespace CloudNet.Application.Features.Auth.Dtos;

public sealed record RefreshTokenResult(
    Guid TokenId,
    Guid FamilyId,
    string RefreshToken,
    string RefreshTokenHash,
    DateTimeOffset RefreshTokenExpiresAt);
