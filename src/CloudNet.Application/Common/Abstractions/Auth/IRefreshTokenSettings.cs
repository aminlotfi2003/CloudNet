namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IRefreshTokenSettings
{
    bool RevokeAllTokensOnReuse { get; }
}
