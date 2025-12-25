namespace CloudNet.Api.Abstractions.Contracts.Folders;

public sealed class CreateFolderRequest
{
    public Guid? ParentId { get; init; }
    public string Name { get; init; } = default!;
}
