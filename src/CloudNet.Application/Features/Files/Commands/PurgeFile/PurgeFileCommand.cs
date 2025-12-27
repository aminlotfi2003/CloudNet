using MediatR;

namespace CloudNet.Application.Features.Files.Commands.PurgeFile;

public sealed record PurgeFileCommand(Guid OwnerId, Guid FileId) : IRequest;
