namespace CloudNet.Web.Services.Models.AuthModels;

public sealed class AuthUserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
}
