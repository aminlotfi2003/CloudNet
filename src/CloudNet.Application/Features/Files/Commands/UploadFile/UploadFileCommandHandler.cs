using AutoMapper;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Domain.Storage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudNet.Application.Features.Files.Commands.UploadFile;

public sealed class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, FileEntryDto>
{
    private readonly IFileEntryRepository _files;
    private readonly IStorageQuotaRepository _quotas;
    private readonly IFileStorage _storage;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _clock;
    private readonly IStorageQuotaSettings _quotaSettings;
    private readonly ILogger<UploadFileCommandHandler> _logger;

    public UploadFileCommandHandler(
        IFileEntryRepository files,
        IStorageQuotaRepository quotas,
        IFileStorage storage,
        IUnitOfWork uow,
        IMapper mapper,
        IDateTimeProvider clock,
        IStorageQuotaSettings quotaSettings,
        ILogger<UploadFileCommandHandler> logger)
    {
        _files = files;
        _quotas = quotas;
        _storage = storage;
        _uow = uow;
        _mapper = mapper;
        _clock = clock;
        _quotaSettings = quotaSettings;
        _logger = logger;
    }

    public async Task<FileEntryDto> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "UploadStarted for owner {OwnerId} in folder {FolderId} size {SizeBytes}",
            request.OwnerId,
            request.FolderId,
            request.SizeBytes);

        var (quota, isNewQuota) = await EnsureQuotaAsync(request.OwnerId, cancellationToken);

        var projectedUsage = quota.UsedBytes + request.SizeBytes;
        if (projectedUsage > quota.QuotaBytes)
            throw new BusinessRuleViolationException("Storage quota exceeded.");

        var storageKey = FileStorageKeyGenerator.Generate(request.OwnerId, request.FileName);

        await _storage.SaveAsync(storageKey, request.Content, cancellationToken);

        var file = new FileEntry
        {
            OwnerId = request.OwnerId,
            FolderId = request.FolderId,
            FileName = request.FileName.Trim(),
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            StoragePath = storageKey,
            Description = request.Description,
            CreatedAt = _clock.UtcNow,
            ModifiedAt = null,
            IsDeleted = false,
            DeletedAt = null
        };

        try
        {
            await _files.AddAsync(file, cancellationToken);

            quota.UsedBytes = projectedUsage;
            quota.UpdatedAt = _clock.UtcNow;
            if (!isNewQuota)
                _quotas.Update(quota);

            await _uow.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _storage.DeleteAsync(storageKey, cancellationToken);
            throw;
        }

        _logger.LogInformation(
            "UploadCompleted for owner {OwnerId} file {FileId} folder {FolderId} size {SizeBytes} storageKey {StorageKey}",
            file.OwnerId,
            file.Id,
            file.FolderId,
            file.SizeBytes,
            file.StoragePath);

        return _mapper.Map<FileEntryDto>(file);
    }

    private async Task<(StorageQuota quota, bool isNew)> EnsureQuotaAsync(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var quota = await _quotas.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (quota is not null)
            return (quota, false);

        if (_quotaSettings.DefaultQuotaBytes <= 0)
            throw new BusinessRuleViolationException("Storage quota default is not configured.");

        var usedBytes = await _files.GetTotalSizeByOwnerAsync(ownerId, cancellationToken);

        quota = new StorageQuota
        {
            OwnerId = ownerId,
            QuotaBytes = _quotaSettings.DefaultQuotaBytes,
            UsedBytes = usedBytes,
            UpdatedAt = _clock.UtcNow
        };

        await _quotas.AddAsync(quota, cancellationToken);

        return (quota, true);
    }
}
