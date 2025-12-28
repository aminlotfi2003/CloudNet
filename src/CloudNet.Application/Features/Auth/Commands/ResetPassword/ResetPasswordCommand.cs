using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Identifier,
    string ResetToken,
    string NewPassword) : IRequest;
