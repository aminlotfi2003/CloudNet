using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Domain.Identity;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CloudNet.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordPolicyService _passwordPolicy;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IPasswordPolicyService passwordPolicy,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _passwordPolicy = passwordPolicy;
        _logger = logger;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await FindByIdentifierAsync(request.Identifier);
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        await EnsurePasswordPolicyAsync(user, request.NewPassword, cancellationToken);

        var decodedToken = DecodeResetToken(request.ResetToken);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        if (!result.Succeeded)
        {
            HandleIdentityErrors(result.Errors);
        }

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            await _passwordPolicy.RecordPasswordChangeAsync(user, user.PasswordHash, cancellationToken);
        }

        _logger.LogInformation("Password reset succeeded for user {UserId}", user.Id);
    }

    private async Task<ApplicationUser?> FindByIdentifierAsync(string identifier)
    {
        var normalized = identifier.Trim();
        if (Guid.TryParse(normalized, out var userId))
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        return await _userManager.FindByEmailAsync(normalized);
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

    private static string DecodeResetToken(string token)
    {
        try
        {
            var bytes = WebEncoders.Base64UrlDecode(token);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("ResetToken", "Reset token is invalid or expired.")
            });
        }
    }

    private static void HandleIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var errorList = errors.ToList();
        if (errorList.Any(e => e.Code == "InvalidToken"))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("ResetToken", "Reset token is invalid or expired.")
            });
        }

        var failures = errorList
            .Select(e => new ValidationFailure("NewPassword", e.Description))
            .ToList();

        throw new ValidationException(failures);
    }
}
