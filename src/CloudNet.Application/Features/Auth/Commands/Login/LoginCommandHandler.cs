using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CloudNet.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow,
        IDateTimeProvider clock)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokens = refreshTokens;
        _uow = uow;
        _clock = clock;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await FindByIdentifierAsync(request.Identifier);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("User is inactive.");
        }

        var signIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (signIn.IsLockedOut)
        {
            throw new ForbiddenException("User is locked out.");
        }

        if (signIn.IsNotAllowed)
        {
            throw new ForbiddenException("User is not allowed to sign in.");
        }

        if (!signIn.Succeeded)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var access = _jwtTokenService.GenerateAccessToken(user, roles);
        var refresh = _refreshTokenService.GenerateRefreshToken();

        var refreshEntity = new RefreshToken
        {
            UserId = user.Id,
            FamilyId = refresh.FamilyId,
            TokenHash = refresh.RefreshTokenHash,
            ExpiresAt = refresh.RefreshTokenExpiresAt,
            CreatedAt = _clock.UtcNow,
            Device = request.Device
        };

        await _refreshTokens.AddAsync(refreshEntity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            new AuthUserDto(user.Id, user.Email ?? string.Empty, user.UserName ?? string.Empty),
            new AuthTokensDto(access.AccessToken, access.AccessTokenExpiresAt, refresh.RefreshToken, refresh.RefreshTokenExpiresAt));
    }

    private async Task<ApplicationUser?> FindByIdentifierAsync(string identifier)
    {
        var normalized = identifier.Trim();
        var user = await _userManager.FindByEmailAsync(normalized);
        if (user is not null)
        {
            return user;
        }

        return await _userManager.FindByNameAsync(normalized);
    }
}
