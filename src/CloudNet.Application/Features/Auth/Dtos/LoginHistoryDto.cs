namespace CloudNet.Application.Features.Auth.Dtos;

public sealed record LoginHistoryDto(
    DateTimeOffset LoggedInAt,
    string? IpAddress,
    string? UserAgent,
    bool IsSuccessful);
