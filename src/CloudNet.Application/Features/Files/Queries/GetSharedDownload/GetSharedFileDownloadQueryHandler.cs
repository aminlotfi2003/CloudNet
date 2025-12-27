using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Files.Dtos;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace CloudNet.Application.Features.Files.Queries.GetSharedDownload;

public sealed class GetSharedFileDownloadQueryHandler : IRequestHandler<GetSharedFileDownloadQuery, FileDownloadDto>
{
    private readonly IShareLinkRepository _links;
    private readonly IFileEntryRepository _files;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public GetSharedFileDownloadQueryHandler(
        IShareLinkRepository links,
        IFileEntryRepository files,
        IUnitOfWork uow,
        IDateTimeProvider clock)
    {
        _links = links;
        _files = files;
        _uow = uow;
        _clock = clock;
    }

    public async Task<FileDownloadDto> Handle(GetSharedFileDownloadQuery request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.Token);
        var link = await _links.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (link is null)
            throw new NotFoundException("Share link not found.");

        if (link.IsRevoked)
            throw new NotFoundException("Share link not found.");

        if (link.ExpiresAt is not null && link.ExpiresAt <= _clock.UtcNow)
            throw new NotFoundException("Share link not found.");

        if (link.MaxDownloads is not null && link.DownloadsCount >= link.MaxDownloads)
            throw new NotFoundException("Share link not found.");

        var file = await _files.GetByIdAsync(link.FileId, cancellationToken);
        if (file is null || file.OwnerId != link.OwnerId)
            throw new NotFoundException("File not found.");

        link.DownloadsCount += 1;
        _links.Update(link);
        await _uow.SaveChangesAsync(cancellationToken);

        return new FileDownloadDto
        {
            StoragePath = file.StoragePath,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.SizeBytes
        };
    }

    private static string HashToken(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
