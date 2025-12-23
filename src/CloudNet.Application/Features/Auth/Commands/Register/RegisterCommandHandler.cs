using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Exceptions;
using MediatR;

namespace CloudNet.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IIdentityService _identity;

    public RegisterCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task Handle(RegisterCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var (ok, errors) = await _identity.CreateUserAsync(dto.UserName, dto.Email, dto.Password, ct);
        if (!ok)
            throw new ConflictException(string.Join(" | ", errors));
    }
}
