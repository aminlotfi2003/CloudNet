using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Identifier,
    string Password,
    string? Device) : IRequest<AuthResponseDto>;
