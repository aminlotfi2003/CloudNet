using CloudNet.Application.Features.Auth.Dtos;

namespace CloudNet.Api.Abstractions.Contracts.Auth;

public sealed class LoginHistoryResponse
{
    public DateTimeOffset LoggedInAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsSuccessful { get; set; }

    public static LoginHistoryResponse Map(LoginHistoryDto dto)
        => new()
        {
            LoggedInAt = dto.LoggedInAt,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent,
            IsSuccessful = dto.IsSuccessful
        };
}
