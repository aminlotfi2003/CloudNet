using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.ListDeleted;

public sealed record ListDeletedFilesQuery(Guid OwnerId) : IRequest<IReadOnlyList<FileEntryDto>>;
