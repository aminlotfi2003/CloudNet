using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenService _refresh;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _uow;

    public LogoutCommandHandler(IRefreshTokenService refresh, IRefreshTokenRepository refreshRepo, IUnitOfWork uow)
    {
        _refresh = refresh;
        _refreshRepo = refreshRepo;
        _uow = uow;
    }

    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return;

        var hash = _refresh.HashToken(request.RefreshToken);
        var token = await _refreshRepo.GetByTokenHashAsync(hash, ct);
        if (token is null || token.IsRevoked)
            return;

        token.IsRevoked = true;
        _refreshRepo.Update(token);
        await _uow.SaveChangesAsync(ct);
    }
}
