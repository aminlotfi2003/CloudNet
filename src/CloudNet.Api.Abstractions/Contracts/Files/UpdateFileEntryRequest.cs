namespace CloudNet.Api.Abstractions.Contracts.Files;

public sealed class UpdateFileEntryRequest
{
    public string? FileName { get; init; }
    public string? Description { get; init; }
}
