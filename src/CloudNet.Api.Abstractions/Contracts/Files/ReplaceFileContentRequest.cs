using Microsoft.AspNetCore.Http;

namespace CloudNet.Api.Abstractions.Contracts.Files;

public sealed class ReplaceFileContentRequest
{
    public IFormFile File { get; init; } = default!;
}
