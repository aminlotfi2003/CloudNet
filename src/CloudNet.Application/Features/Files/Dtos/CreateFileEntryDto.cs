namespace CloudNet.Application.Features.Files.Dtos;

public sealed class CreateFileEntryDto
{
    public Guid OwnerId { get; init; }
    public Guid FolderId { get; init; }

    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long SizeBytes { get; init; }

    public string StoragePath { get; init; } = default!;
    public string? Description { get; init; }
}
