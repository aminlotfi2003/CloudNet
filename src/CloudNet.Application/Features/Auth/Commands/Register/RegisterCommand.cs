using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(RegisterRequestDto Dto) : IRequest;
