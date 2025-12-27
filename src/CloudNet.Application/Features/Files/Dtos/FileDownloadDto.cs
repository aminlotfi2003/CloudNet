namespace CloudNet.Application.Features.Files.Dtos;

public sealed class FileDownloadDto
{
    public string StoragePath { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long SizeBytes { get; init; }
}
