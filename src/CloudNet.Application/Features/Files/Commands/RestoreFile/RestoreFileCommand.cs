using MediatR;

namespace CloudNet.Application.Features.Files.Commands.RestoreFile;

public sealed record RestoreFileCommand(Guid OwnerId, Guid FileId) : IRequest;
