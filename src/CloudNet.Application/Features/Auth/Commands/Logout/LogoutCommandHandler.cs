using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudNet.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow,
        IDateTimeProvider clock,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenService = refreshTokenService;
        _refreshTokens = refreshTokens;
        _uow = uow;
        _clock = clock;
        _logger = logger;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
        var storedToken = await _refreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || storedToken.UserId != request.UserId)
        {
            _logger.LogInformation("Logout ignored for user {UserId}", request.UserId);
            return;
        }

        var now = _clock.UtcNow;
        await _refreshTokens.RevokeFamilyTokensAsync(storedToken.FamilyId, now, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Logout completed for user {UserId}", request.UserId);
    }
}
