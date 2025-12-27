using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CloudNet.Infrastructure.Persistence.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorage(IOptions<FileStorageOptions> options, IHostEnvironment environment)
    {
        var configuredPath = options.Value.RootPath;
        if (string.IsNullOrWhiteSpace(configuredPath))
            throw new InvalidOperationException("File storage root path is not configured.");

        var basePath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);

        _rootPath = Path.GetFullPath(basePath);
        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveAsync(string storageKey, Stream content, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var output = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);

        await content.CopyToAsync(output, cancellationToken);
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        return Task.FromResult(File.Exists(fullPath));
    }

    private string GetFullPath(string storageKey)
    {
        var normalizedKey = storageKey.Replace('/', Path.DirectorySeparatorChar);
        var combined = Path.Combine(_rootPath, normalizedKey);
        var fullPath = Path.GetFullPath(combined);

        if (!fullPath.StartsWith(_rootPath, StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid storage key.");

        return fullPath;
    }
}
