using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.SoftDeleteFolder;

public sealed record SoftDeleteFolderCommand(Guid OwnerId, Guid FolderId) : IRequest;
