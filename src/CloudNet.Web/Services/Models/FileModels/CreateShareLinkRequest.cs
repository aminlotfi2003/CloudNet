namespace CloudNet.Web.Services.Models.FileModels;

public sealed class CreateShareLinkRequest
{
    public DateTimeOffset? ExpiresAt { get; init; }
    public int? MaxDownloads { get; init; }
}
