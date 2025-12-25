namespace CloudNet.Api.Abstractions.Contracts.Auth;

public sealed class AuthUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
