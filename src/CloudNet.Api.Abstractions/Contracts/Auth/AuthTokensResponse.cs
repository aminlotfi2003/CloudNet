using CloudNet.Application.Features.Auth.Dtos;

namespace CloudNet.Api.Abstractions.Contracts.Auth;

public sealed class AuthTokensResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public static AuthTokensResponse MapTokensResponse(AuthTokensDto dto)
        => new()
        {
            AccessToken = dto.AccessToken,
            AccessTokenExpiresAt = dto.AccessTokenExpiresAt,
            RefreshToken = dto.RefreshToken,
            RefreshTokenExpiresAt = dto.RefreshTokenExpiresAt
        };
}
