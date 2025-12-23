using CloudNet.Application.Features.Auth.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(LoginRequestDto Dto) : IRequest<AuthTokensDto>;
