namespace CloudNet.Web.Services.Models.FileModels;

public sealed class FileEntryDto
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }
    public Guid FolderId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string StoragePath { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
}
