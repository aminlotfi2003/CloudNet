using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    string? Device) : IRequest<AuthResponseDto>;
