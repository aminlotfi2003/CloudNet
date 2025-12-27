namespace CloudNet.Infrastructure.Persistence.Options;

public sealed class StorageQuotaOptions
{
    public const string SectionName = "StorageQuota";

    public long DefaultLimitBytes { get; init; } = 1L * 1024 * 1024 * 1024;
}
