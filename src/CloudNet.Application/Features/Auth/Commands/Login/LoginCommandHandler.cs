using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthTokensDto>
{
    private readonly IIdentityService _identity;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenService _refresh;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _uow;

    public LoginCommandHandler(
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

    public async Task<AuthTokensDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var user = await _identity.FindByUserNameOrEmailAsync(dto.UserNameOrEmail, ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedException("Invalid credentials.");

        var passOk = await _identity.CheckPasswordAsync(user, dto.Password, ct);
        if (!passOk)
            throw new UnauthorizedException("Invalid credentials.");

        var roles = await _identity.GetRolesAsync(user, ct);

        var accessToken = _jwt.CreateAccessToken(user, roles);
        var refreshRaw = _refresh.GenerateRawToken();
        var refreshHash = _refresh.HashToken(refreshRaw);

        await _refreshRepo.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_refresh.RefreshTokenDays),
            IsRevoked = false
        }, ct);

        await _uow.SaveChangesAsync(ct);

        return new AuthTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshRaw
        };
    }
}
