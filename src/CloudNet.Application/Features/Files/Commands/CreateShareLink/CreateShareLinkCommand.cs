using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.CreateShareLink;

public sealed record CreateShareLinkCommand(
    Guid OwnerId,
    Guid FileId,
    DateTimeOffset? ExpiresAt,
    int? MaxDownloads) : IRequest<ShareLinkTokenDto>;
