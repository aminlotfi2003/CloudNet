using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.UploadFile;

public sealed record UploadFileCommand(
    Guid OwnerId,
    Guid FolderId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content,
    string? Description) : IRequest<FileEntryDto>;
