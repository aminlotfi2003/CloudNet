namespace CloudNet.Web.Services.Models.AuthModels;

public sealed class AuthResponse
{
    public AuthUserResponse User { get; init; } = new();
    public AuthTokensResponse Tokens { get; init; } = new();
}
