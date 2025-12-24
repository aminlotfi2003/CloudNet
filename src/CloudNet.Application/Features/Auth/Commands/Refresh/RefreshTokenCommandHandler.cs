using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CloudNet.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenSettings _refreshTokenSettings;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenSettings refreshTokenSettings,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow,
        IDateTimeProvider clock)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenSettings = refreshTokenSettings;
        _refreshTokens = refreshTokens;
        _uow = uow;
        _clock = clock;
    }

    public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
        var storedToken = await _refreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is null)
        {
            throw new UnauthorizedException("Refresh token is invalid.");
        }

        var now = _clock.UtcNow;
        if (!storedToken.IsActive(now))
        {
            if (storedToken.IsRevoked && storedToken.ReplacedByTokenId.HasValue)
            {
                if (_refreshTokenSettings.RevokeAllTokensOnReuse)
                {
                    await _refreshTokens.RevokeUserTokensAsync(storedToken.UserId, now, cancellationToken);
                }
                else
                {
                    await _refreshTokens.RevokeFamilyTokensAsync(storedToken.FamilyId, now, cancellationToken);
                }

                await _uow.SaveChangesAsync(cancellationToken);
            }

            throw new UnauthorizedException("Refresh token is invalid.");
        }

        var user = storedToken.User;
        var roles = await _userManager.GetRolesAsync(user);
        var access = _jwtTokenService.GenerateAccessToken(user, roles);
        var refresh = _refreshTokenService.GenerateRefreshToken(storedToken.FamilyId);

        storedToken.Revoke(now, refresh.TokenId);

        var newToken = new RefreshToken
        {
            UserId = user.Id,
            FamilyId = refresh.FamilyId,
            TokenHash = refresh.RefreshTokenHash,
            ExpiresAt = refresh.RefreshTokenExpiresAt,
            CreatedAt = now,
            Device = request.Device
        };

        await _refreshTokens.AddAsync(newToken, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(
            access.AccessToken,
            access.AccessTokenExpiresAt,
            refresh.RefreshToken,
            refresh.RefreshTokenExpiresAt);
    }
}
