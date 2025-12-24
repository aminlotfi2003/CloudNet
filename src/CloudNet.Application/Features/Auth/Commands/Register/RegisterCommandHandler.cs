using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Domain.Identity;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CloudNet.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordPolicyService _passwordPolicy;
    private readonly IDateTimeProvider _clock;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow,
        IPasswordPolicyService passwordPolicy,
        IDateTimeProvider clock)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokens = refreshTokens;
        _uow = uow;
        _passwordPolicy = passwordPolicy;
        _clock = clock;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email.Trim(),
            UserName = request.UserName.Trim(),
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            HandleIdentityErrors(result.Errors);
        }

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            await _passwordPolicy.RecordPasswordChangeAsync(user, user.PasswordHash, cancellationToken);
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

    private static void HandleIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var errorList = errors.ToList();
        if (errorList.Any(e => e.Code == "DuplicateEmail"))
        {
            throw new ConflictException("Email is already in use.");
        }

        if (errorList.Any(e => e.Code == "DuplicateUserName"))
        {
            throw new ConflictException("Username is already in use.");
        }

        var failures = errorList
            .Select(e => new ValidationFailure("Password", e.Description))
            .ToList();

        throw new ValidationException(failures);
    }
}
