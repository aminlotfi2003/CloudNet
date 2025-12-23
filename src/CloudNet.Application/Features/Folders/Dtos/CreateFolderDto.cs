namespace CloudNet.Application.Features.Folders.Dtos;

public sealed class CreateFolderDto
{
    public Guid OwnerId { get; init; }
    public Guid? ParentId { get; init; }
    public string Name { get; init; } = default!;
}
