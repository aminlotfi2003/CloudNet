using MediatR;

namespace CloudNet.Application.Features.Files.Commands.SoftDeleteFile;

public sealed record SoftDeleteFileCommand(Guid OwnerId, Guid FileId) : IRequest;
