using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.RestoreFolder;

public sealed record RestoreFolderCommand(Guid OwnerId, Guid FolderId) : IRequest;
