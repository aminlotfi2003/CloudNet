using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Queries.ListDeleted;

public sealed record ListDeletedFoldersQuery(Guid OwnerId) : IRequest<IReadOnlyList<FolderDto>>;
