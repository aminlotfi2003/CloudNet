using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.GetDownload;

public sealed record GetFileDownloadQuery(Guid OwnerId, Guid FileId) : IRequest<FileDownloadDto>;
