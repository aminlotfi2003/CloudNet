namespace CloudNet.Application.Features.Files.Dtos;

public sealed class UpdateFileEntryDto
{
    public Guid FileId { get; init; }
    public Guid OwnerId { get; init; }

    // Optional updates
    public string? FileName { get; init; }
    public string? Description { get; init; }
}
