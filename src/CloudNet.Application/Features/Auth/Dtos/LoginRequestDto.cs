namespace CloudNet.Application.Features.Auth.Dtos;

public sealed class LoginRequestDto
{
    public string UserNameOrEmail { get; init; } = default!;
    public string Password { get; init; } = default!;
}
