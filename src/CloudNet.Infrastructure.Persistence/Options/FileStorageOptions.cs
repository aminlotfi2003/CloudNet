namespace CloudNet.Infrastructure.Persistence.Options;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string RootPath { get; init; } = "storage";
}
