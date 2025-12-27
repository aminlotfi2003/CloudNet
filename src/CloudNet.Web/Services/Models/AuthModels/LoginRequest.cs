namespace CloudNet.Web.Services.Models.AuthModels;

public sealed class LoginRequest
{
    public string Identifier { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
