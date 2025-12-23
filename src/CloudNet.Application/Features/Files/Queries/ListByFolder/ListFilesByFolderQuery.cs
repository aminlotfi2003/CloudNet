using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.ListByFolder;

public sealed record ListFilesByFolderQuery(Guid OwnerId, Guid FolderId) : IRequest<IReadOnlyList<FileEntryDto>>;
