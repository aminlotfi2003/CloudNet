using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CloudNet.Application.Features.Auth.Commands.ForgotPassword;


public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IDateTimeProvider clock,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _clock = clock;
        _logger = logger;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await FindByIdentifierAsync(request.Identifier);
        if (user is null || !user.IsActive)
        {
            _logger.LogInformation(
                "Password reset requested for identifier {Identifier} at {Timestamp}",
                request.Identifier,
                _clock.UtcNow);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _logger.LogInformation(
            "Generated password reset token for user {UserId} at {Timestamp}",
            user.Id,
            _clock.UtcNow);

        _ = encodedToken;
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
