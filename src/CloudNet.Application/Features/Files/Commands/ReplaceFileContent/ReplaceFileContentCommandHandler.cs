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

namespace CloudNet.Application.Features.Files.Commands.ReplaceFileContent;

public sealed class ReplaceFileContentCommandHandler : IRequestHandler<ReplaceFileContentCommand, FileEntryDto>
{
    private readonly IFileEntryRepository _files;
    private readonly IStorageQuotaRepository _quotas;
    private readonly IFileStorage _storage;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _clock;
    private readonly IStorageQuotaSettings _quotaSettings;
    private readonly ILogger<ReplaceFileContentCommandHandler> _logger;

    public ReplaceFileContentCommandHandler(
        IFileEntryRepository files,
        IStorageQuotaRepository quotas,
        IFileStorage storage,
        IUnitOfWork uow,
        IMapper mapper,
        IDateTimeProvider clock,
        IStorageQuotaSettings quotaSettings,
        ILogger<ReplaceFileContentCommandHandler> logger)
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

    public async Task<FileEntryDto> Handle(ReplaceFileContentCommand request, CancellationToken cancellationToken)
    {
        var file = await _files.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null || file.OwnerId != request.OwnerId)
            throw new NotFoundException("File not found.");

        var (quota, isNewQuota) = await EnsureQuotaAsync(request.OwnerId, cancellationToken);

        var sizeDelta = request.SizeBytes - file.SizeBytes;
        if (sizeDelta > 0 && quota.UsedBytes + sizeDelta > quota.QuotaBytes)
            throw new BusinessRuleViolationException("Storage quota exceeded.");

        var newStorageKey = FileStorageKeyGenerator.Generate(request.OwnerId, request.FileName);
        await _storage.SaveAsync(newStorageKey, request.Content, cancellationToken);

        var oldStorageKey = file.StoragePath;

        file.StoragePath = newStorageKey;
        file.ContentType = request.ContentType;
        file.SizeBytes = request.SizeBytes;
        file.ModifiedAt = _clock.UtcNow;

        try
        {
            _files.Update(file);

            quota.UsedBytes += sizeDelta;
            quota.UpdatedAt = _clock.UtcNow;
            if (!isNewQuota)
                _quotas.Update(quota);

            await _uow.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _storage.DeleteAsync(newStorageKey, cancellationToken);
            throw;
        }

        await _storage.DeleteAsync(oldStorageKey, cancellationToken);

        _logger.LogInformation(
            "ReplaceCompleted for owner {OwnerId} file {FileId} size {SizeBytes} storageKey {StorageKey}",
            file.OwnerId,
            file.Id,
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
