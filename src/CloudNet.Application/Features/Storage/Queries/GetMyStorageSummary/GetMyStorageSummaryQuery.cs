using CloudNet.Application.Features.Storage.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Storage.Queries.GetMyStorageSummary;

public sealed record GetMyStorageSummaryQuery(Guid OwnerId) : IRequest<StorageSummaryDto>;
