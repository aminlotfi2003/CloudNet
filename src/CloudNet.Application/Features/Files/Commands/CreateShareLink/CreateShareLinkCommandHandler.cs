using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Domain.Enums;
using CloudNet.Domain.Storage;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace CloudNet.Application.Features.Files.Commands.CreateShareLink;

public sealed class CreateShareLinkCommandHandler : IRequestHandler<CreateShareLinkCommand, ShareLinkTokenDto>
{
    private readonly IFileEntryRepository _files;
    private readonly IShareLinkRepository _links;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public CreateShareLinkCommandHandler(
        IFileEntryRepository files,
        IShareLinkRepository links,
        IUnitOfWork uow,
        IDateTimeProvider clock)
    {
        _files = files;
        _links = links;
        _uow = uow;
        _clock = clock;
    }

    public async Task<ShareLinkTokenDto> Handle(CreateShareLinkCommand request, CancellationToken cancellationToken)
    {
        var file = await _files.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null || file.OwnerId != request.OwnerId)
            throw new NotFoundException("File not found.");

        var token = GenerateToken();
        var tokenHash = HashToken(token);

        var link = new ShareLink
        {
            OwnerId = request.OwnerId,
            FileId = request.FileId,
            TokenHash = tokenHash,
            CreatedAt = _clock.UtcNow,
            ExpiresAt = request.ExpiresAt,
            MaxDownloads = request.MaxDownloads,
            DownloadsCount = 0,
            IsRevoked = false,
            Permission = SharePermission.ReadOnly
        };

        await _links.AddAsync(link, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ShareLinkTokenDto
        {
            ShareLinkId = link.Id,
            FileId = link.FileId,
            Token = token,
            ExpiresAt = link.ExpiresAt,
            MaxDownloads = link.MaxDownloads
        };
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var base64 = Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        return base64;
    }

    private static string HashToken(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
