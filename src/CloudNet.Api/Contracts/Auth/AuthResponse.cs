using Microsoft.AspNetCore.Authentication.OAuth;

namespace CloudNet.Api.Contracts.Auth;

public sealed class AuthResponse
{
    public AuthUserResponse User { get; set; } = new();
    public AuthTokensResponse Tokens { get; set; } = new();
}
