namespace CloudNet.Application.Features.Files.Dtos;

public sealed class ShareLinkTokenDto
{
    public Guid ShareLinkId { get; init; }
    public Guid FileId { get; init; }
    public string Token { get; init; } = default!;
    public DateTimeOffset? ExpiresAt { get; init; }
    public int? MaxDownloads { get; init; }
}
