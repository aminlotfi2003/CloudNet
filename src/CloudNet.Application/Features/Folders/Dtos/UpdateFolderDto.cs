namespace CloudNet.Application.Features.Folders.Dtos;

public sealed class UpdateFolderDto
{
    public Guid FolderId { get; init; }
    public Guid OwnerId { get; init; }
    public string Name { get; init; } = default!;
}
