using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;
