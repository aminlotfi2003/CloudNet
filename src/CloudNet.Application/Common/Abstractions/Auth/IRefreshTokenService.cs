namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IRefreshTokenService
{
    string GenerateRawToken();
    string HashToken(string rawToken);
    int RefreshTokenDays { get; }
}
