using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.UpdateFolder;

public sealed record UpdateFolderCommand(UpdateFolderDto Dto) : IRequest<FolderDto>;
