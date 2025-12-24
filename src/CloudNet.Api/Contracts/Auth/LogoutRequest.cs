namespace CloudNet.Api.Contracts.Auth;

public sealed class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
