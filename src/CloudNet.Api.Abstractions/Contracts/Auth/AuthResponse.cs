using CloudNet.Application.Features.Auth.Dtos;

namespace CloudNet.Api.Abstractions.Contracts.Auth;

public sealed class AuthResponse
{
    public AuthUserResponse User { get; set; } = new();
    public AuthTokensResponse Tokens { get; set; } = new();

    public static AuthResponse MapAuthResponse(AuthResponseDto dto)
        => new()
        {
            User = new AuthUserResponse
            {
                Id = dto.User.Id,
                Email = dto.User.Email,
                UserName = dto.User.UserName
            },
            Tokens = AuthTokensResponse.MapTokensResponse(dto.Tokens)
        };
}
