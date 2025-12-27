namespace CloudNet.Web.Services.Models.AuthModels;

public sealed class LogoutRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
