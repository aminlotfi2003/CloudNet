namespace CloudNet.Web.Services.Models.FileModels;

public sealed class CreateFileEntryRequest
{
    public Guid FolderId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string StoragePath { get; init; } = string.Empty;
    public string? Description { get; init; }
}
