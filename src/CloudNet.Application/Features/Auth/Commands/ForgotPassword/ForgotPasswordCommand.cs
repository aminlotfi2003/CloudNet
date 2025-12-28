using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Identifier) : IRequest;
