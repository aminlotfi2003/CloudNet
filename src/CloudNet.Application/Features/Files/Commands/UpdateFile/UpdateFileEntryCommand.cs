using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.UpdateFile;

public sealed record UpdateFileEntryCommand(UpdateFileEntryDto Dto) : IRequest<FileEntryDto>;
