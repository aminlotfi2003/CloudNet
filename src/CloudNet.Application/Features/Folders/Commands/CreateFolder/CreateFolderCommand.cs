using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.CreateFolder;

public sealed record CreateFolderCommand(CreateFolderDto Dto) : IRequest<FolderDto>;
