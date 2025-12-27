using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.ReplaceFileContent;

public sealed record ReplaceFileContentCommand(
    Guid OwnerId,
    Guid FileId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content) : IRequest<FileEntryDto>;
