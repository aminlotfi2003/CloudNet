using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Domain.Identity;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CloudNet.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordPolicyService _passwordPolicy;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IPasswordPolicyService passwordPolicy,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork unitOfWork,
        IDateTimeProvider clock,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _passwordPolicy = passwordPolicy;
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _logger = logger;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("User is inactive.");
        }

        await EnsurePasswordPolicyAsync(user, request.NewPassword, cancellationToken);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            HandleIdentityErrors(result.Errors);
        }

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            await _passwordPolicy.RecordPasswordChangeAsync(user, user.PasswordHash, cancellationToken);
        }

        await _refreshTokens.RevokeUserTokensAsync(user.Id, _clock.UtcNow, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password change succeeded for user {UserId}", user.Id);
    }

    private async Task EnsurePasswordPolicyAsync(ApplicationUser user, string password, CancellationToken cancellationToken)
    {
        try
        {
            await _passwordPolicy.EnsurePasswordCompliesAsync(user, password, cancellationToken);
        }
        catch (BusinessRuleViolationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure("NewPassword", ex.Message) });
        }
    }

    private static void HandleIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var errorList = errors.ToList();
        if (errorList.Any(e => e.Code == "PasswordMismatch"))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("CurrentPassword", "Current password is incorrect.")
            });
        }

        var failures = errorList
            .Select(e => new ValidationFailure("NewPassword", e.Description))
            .ToList();

        throw new ValidationException(failures);
    }
}
