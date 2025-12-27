namespace CloudNet.Web.Services.Models.FolderModels;

public sealed class CreateFolderRequest
{
    public Guid? ParentId { get; init; }
    public string Name { get; init; } = string.Empty;
}
