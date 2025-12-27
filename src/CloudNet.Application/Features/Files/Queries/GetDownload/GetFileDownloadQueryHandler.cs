using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.GetDownload;

public sealed class GetFileDownloadQueryHandler : IRequestHandler<GetFileDownloadQuery, FileDownloadDto>
{
    private readonly IFileEntryRepository _files;

    public GetFileDownloadQueryHandler(IFileEntryRepository files)
    {
        _files = files;
    }

    public async Task<FileDownloadDto> Handle(GetFileDownloadQuery request, CancellationToken cancellationToken)
    {
        var file = await _files.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null || file.OwnerId != request.OwnerId)
            throw new NotFoundException("File not found.");

        return new FileDownloadDto
        {
            StoragePath = file.StoragePath,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.SizeBytes
        };
    }
}
