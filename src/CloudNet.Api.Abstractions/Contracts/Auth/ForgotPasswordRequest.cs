namespace CloudNet.Api.Abstractions.Contracts.Auth;

public sealed class ForgotPasswordRequest
{
    public string Identifier { get; set; } = string.Empty;
}
