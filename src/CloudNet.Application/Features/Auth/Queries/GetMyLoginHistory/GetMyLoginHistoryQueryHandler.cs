using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Queries.GetMyLoginHistory;

public sealed class GetMyLoginHistoryQueryHandler : IRequestHandler<GetMyLoginHistoryQuery, IReadOnlyList<LoginHistoryDto>>
{
    private readonly ILoginHistoryRepository _loginHistory;

    public GetMyLoginHistoryQueryHandler(ILoginHistoryRepository loginHistory)
    {
        _loginHistory = loginHistory;
    }

    public async Task<IReadOnlyList<LoginHistoryDto>> Handle(GetMyLoginHistoryQuery request, CancellationToken cancellationToken)
    {
        var history = await _loginHistory.GetRecentAsync(request.UserId, 10, cancellationToken);

        return history
            .Select(entry => new LoginHistoryDto(
                entry.OccurredAt,
                entry.IpAddress,
                entry.UserAgent,
                entry.Success))
            .ToList();
    }
}
