namespace CloudNet.Application.Features.Storage.Dtos;

public sealed class StorageSummaryDto
{
    public Guid OwnerId { get; init; }
    public long LimitBytes { get; init; }
    public long UsedBytes { get; init; }
    public long FreeBytes { get; init; }
    public decimal UsedPercent { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}
