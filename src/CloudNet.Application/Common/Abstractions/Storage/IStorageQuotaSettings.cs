namespace CloudNet.Application.Common.Abstractions.Storage;

public interface IStorageQuotaSettings
{
    long DefaultQuotaBytes { get; }
}
