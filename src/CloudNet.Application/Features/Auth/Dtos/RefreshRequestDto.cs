namespace CloudNet.Application.Features.Auth.Dtos;

public sealed class RefreshRequestDto
{
    public string RefreshToken { get; init; } = default!;
}
