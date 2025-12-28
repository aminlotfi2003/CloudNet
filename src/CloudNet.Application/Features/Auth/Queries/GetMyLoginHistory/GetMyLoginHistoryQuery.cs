using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Queries.GetMyLoginHistory;

public sealed record GetMyLoginHistoryQuery(Guid UserId) : IRequest<IReadOnlyList<LoginHistoryDto>>;
