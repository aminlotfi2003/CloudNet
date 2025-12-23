namespace CloudNet.Application.Features.Auth.Dtos;

public sealed class RegisterRequestDto
{
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
