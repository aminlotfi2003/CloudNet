namespace CloudNet.Api.Contracts.Files;

public sealed class CreateFileEntryRequest
{
    public Guid FolderId { get; init; }

    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long SizeBytes { get; init; }

    public string StoragePath { get; init; } = default!;
    public string? Description { get; init; }
}
