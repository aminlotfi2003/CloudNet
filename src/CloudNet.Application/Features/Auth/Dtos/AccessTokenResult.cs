namespace CloudNet.Application.Features.Auth.Dtos;

public sealed record AccessTokenResult(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt);
