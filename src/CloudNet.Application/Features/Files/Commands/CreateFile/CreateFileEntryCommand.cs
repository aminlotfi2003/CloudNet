using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.CreateFile;

public sealed record CreateFileEntryCommand(CreateFileEntryDto Dto) : IRequest<FileEntryDto>;
