using CloudNet.Application.Features.Auth.Dtos;

namespace CloudNet.Application.Common.Abstractions.Auth;

public interface IRefreshTokenService
{
    RefreshTokenResult GenerateRefreshToken(Guid? familyId = null);
    string HashToken(string value);
}
