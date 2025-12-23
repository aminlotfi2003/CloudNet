using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Queries.ListChildren;

public sealed record ListFolderChildrenQuery(Guid OwnerId, Guid? ParentId) : IRequest<IReadOnlyList<FolderDto>>;
