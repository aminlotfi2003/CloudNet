namespace CloudNet.Application.Common.Abstractions.Storage;

public static class FileStorageKeyGenerator
{
    public static string Generate(Guid ownerId, string? originalFileName)
    {
        var extension = Path.GetExtension(originalFileName ?? string.Empty);
        var fileId = Guid.NewGuid().ToString("N");
        return $"{ownerId:D}/{fileId}{extension}";
    }
}
