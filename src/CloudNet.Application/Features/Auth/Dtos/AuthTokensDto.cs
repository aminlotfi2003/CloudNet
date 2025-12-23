namespace CloudNet.Application.Features.Auth.Dtos;

public sealed class AuthTokensDto
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
}
