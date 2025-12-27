using Microsoft.AspNetCore.Http;

namespace CloudNet.Api.Abstractions.Contracts.Files;

public sealed class UploadFileRequest
{
    public Guid FolderId { get; init; }
    public IFormFile File { get; init; } = default!;
    public string? Description { get; init; }
}
