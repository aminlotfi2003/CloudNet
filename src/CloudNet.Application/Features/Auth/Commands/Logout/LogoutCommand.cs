using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(
    Guid UserId,
    string RefreshToken) : IRequest;
