using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Options;

namespace CloudNet.Infrastructure.Persistence.Storage;

public sealed class StorageQuotaSettings : IStorageQuotaSettings
{
    private readonly StorageQuotaOptions _options;

    public StorageQuotaSettings(IOptions<StorageQuotaOptions> options)
    {
        _options = options.Value;
    }

    public long DefaultQuotaBytes => _options.DefaultLimitBytes;
}
