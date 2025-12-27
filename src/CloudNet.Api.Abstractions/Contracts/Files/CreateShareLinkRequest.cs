namespace CloudNet.Api.Abstractions.Contracts.Files;

public sealed class CreateShareLinkRequest
{
    public DateTimeOffset? ExpiresAt { get; init; }
    public int? MaxDownloads { get; init; }
}
