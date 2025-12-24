namespace CloudNet.Application.Features.Auth.Dtos;

public sealed record AuthUserDto(
    Guid Id,
    string Email,
    string UserName);
