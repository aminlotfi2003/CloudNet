namespace CloudNet.Web.Services.Models.FileModels;

public sealed class UpdateFileEntryRequest
{
    public string FileName { get; init; } = string.Empty;
    public string? Description { get; init; }
}
