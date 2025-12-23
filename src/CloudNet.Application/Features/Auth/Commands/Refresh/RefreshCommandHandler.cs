using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthTokensDto>
{
    private readonly IIdentityService _identity;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenService _refresh;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _uow;

    public RefreshCommandHandler(
        IIdentityService identity,
        IJwtTokenService jwt,
        IRefreshTokenService refresh,
        IRefreshTokenRepository refreshRepo,
        IUnitOfWork uow)
    {
        _identity = identity;
        _jwt = jwt;
        _refresh = refresh;
        _refreshRepo = refreshRepo;
        _uow = uow;
    }

    public async Task<AuthTokensDto> Handle(RefreshCommand request, CancellationToken ct)
    {
        var raw = request.Dto.RefreshToken;
        if (string.IsNullOrWhiteSpace(raw))
            throw new UnauthorizedException("Invalid refresh token.");

        var hash = _refresh.HashToken(raw);

        var oldToken = await _refreshRepo.GetByTokenHashAsync(hash, ct);
        if (oldToken is null || oldToken.IsRevoked || oldToken.ExpiresAt <= DateTimeOffset.UtcNow)
            throw new UnauthorizedException("Invalid refresh token.");

        var user = await _identity.FindByIdAsync(oldToken.UserId, ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedException("Invalid refresh token.");

        var roles = await _identity.GetRolesAsync(user, ct);

        // ROTATION: revoke old token
        oldToken.IsRevoked = true;
        _refreshRepo.Update(oldToken);

        // issue new tokens
        var newAccess = _jwt.CreateAccessToken(user, roles);

        var newRefreshRaw = _refresh.GenerateRawToken();
        var newRefreshHash = _refresh.HashToken(newRefreshRaw);

        await _refreshRepo.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_refresh.RefreshTokenDays),
            IsRevoked = false
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return new AuthTokensDto
        {
            AccessToken = newAccess,
            RefreshToken = newRefreshRaw
        };
    }
}
