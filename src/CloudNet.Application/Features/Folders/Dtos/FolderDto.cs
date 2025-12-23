namespace CloudNet.Application.Features.Folders.Dtos;

public sealed class FolderDto
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }
    public Guid? ParentId { get; init; }
    public string Name { get; init; } = default!;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
}
