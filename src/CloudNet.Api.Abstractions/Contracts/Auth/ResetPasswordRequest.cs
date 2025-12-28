namespace CloudNet.Api.Abstractions.Contracts.Auth;

public sealed class ResetPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
