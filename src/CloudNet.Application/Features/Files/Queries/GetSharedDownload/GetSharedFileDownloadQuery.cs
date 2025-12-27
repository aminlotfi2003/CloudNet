using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.GetSharedDownload;

public sealed record GetSharedFileDownloadQuery(string Token) : IRequest<FileDownloadDto>;
