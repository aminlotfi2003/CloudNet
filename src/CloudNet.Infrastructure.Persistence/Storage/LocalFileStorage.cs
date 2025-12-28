using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudNet.Infrastructure.Persistence.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;
    private readonly ILogger<LocalFileStorage> _logger;

    public LocalFileStorage(
        IOptions<FileStorageOptions> options, 
        IHostEnvironment environment,
        ILogger<LocalFileStorage> logger)
    {
        _logger = logger;
        var configuredPath = options.Value.RootPath;
        if (string.IsNullOrWhiteSpace(configuredPath))
            throw new InvalidOperationException("File storage root path is not configured.");

        var basePath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);

        _rootPath = Path.GetFullPath(basePath);
        _rootPath = _rootPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
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

        _logger.LogInformation("Stored file content for {StorageKey}", storageKey);
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

        _logger.LogDebug("Opened file content for {StorageKey}", storageKey);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file content for {StorageKey}", storageKey);
        }
        else
        {
            _logger.LogDebug("Delete skipped, file not found for {StorageKey}", storageKey);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        var exists = File.Exists(fullPath);
        _logger.LogDebug("Checked file exists for {StorageKey}: {Exists}", storageKey, exists);
        return Task.FromResult(exists);
    }

    private string GetFullPath(string storageKey)
    {
        var normalizedKey = storageKey.Replace('/', Path.DirectorySeparatorChar);
        var combined = Path.Combine(_rootPath, normalizedKey);
        var fullPath = Path.GetFullPath(combined);

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (!fullPath.StartsWith(_rootPath, comparison))
            throw new InvalidOperationException("Invalid storage key.");

        return fullPath;
    }
}
