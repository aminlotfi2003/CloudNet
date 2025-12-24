using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshTokenCommand(
    string RefreshToken,
    string? Device) : IRequest<AuthTokensDto>;
