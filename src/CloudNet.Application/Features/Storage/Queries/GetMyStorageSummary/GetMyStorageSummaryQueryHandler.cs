using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Storage.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Storage.Queries.GetMyStorageSummary;

public sealed class GetMyStorageSummaryQueryHandler
    : IRequestHandler<GetMyStorageSummaryQuery, StorageSummaryDto>
{
    private readonly IStorageQuotaRepository _quotas;

    public GetMyStorageSummaryQueryHandler(IStorageQuotaRepository quotas)
    {
        _quotas = quotas;
    }

    public async Task<StorageSummaryDto> Handle(GetMyStorageSummaryQuery request, CancellationToken cancellationToken)
    {
        var quota = await _quotas.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
        if (quota is null)
            throw new NotFoundException("Storage quota not found.");

        var freeBytes = Math.Max(quota.QuotaBytes - quota.UsedBytes, 0);
        var usedPercent = CalculateUsedPercent(quota.UsedBytes, quota.QuotaBytes);

        return new StorageSummaryDto
        {
            OwnerId = quota.OwnerId,
            LimitBytes = quota.QuotaBytes,
            UsedBytes = quota.UsedBytes,
            FreeBytes = freeBytes,
            UsedPercent = usedPercent,
            UpdatedAt = quota.UpdatedAt
        };
    }

    private static decimal CalculateUsedPercent(long usedBytes, long limitBytes)
    {
        if (limitBytes <= 0)
            return 0m;

        var percent = (decimal)usedBytes * 100m / limitBytes;
        percent = Math.Clamp(percent, 0m, 100m);
        return Math.Round(percent, 2, MidpointRounding.AwayFromZero);
    }
}
