namespace CloudNet.Web.Services.Models.FileModels;

public sealed class ShareLinkTokenDto
{
    public Guid ShareLinkId { get; init; }
    public Guid FileId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTimeOffset? ExpiresAt { get; init; }
    public int? MaxDownloads { get; init; }
}
