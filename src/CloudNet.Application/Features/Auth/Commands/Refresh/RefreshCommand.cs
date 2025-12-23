using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshCommand(RefreshRequestDto Dto) : IRequest<AuthTokensDto>;
