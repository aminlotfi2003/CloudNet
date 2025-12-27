using Asp.Versioning;
using CloudNet.Api.Abstractions.Constants;
using CloudNet.Api.Abstractions.Contracts.Files;
using CloudNet.Api.Abstractions.Extensions;
using CloudNet.Api.Abstractions.RateLimiting;
using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Application.Features.Files.Commands.CreateFile;
using CloudNet.Application.Features.Files.Commands.CreateShareLink;
using CloudNet.Application.Features.Files.Commands.PurgeFile;
using CloudNet.Application.Features.Files.Commands.ReplaceFileContent;
using CloudNet.Application.Features.Files.Commands.RestoreFile;
using CloudNet.Application.Features.Files.Commands.SoftDeleteFile;
using CloudNet.Application.Features.Files.Commands.UpdateFile;
using CloudNet.Application.Features.Files.Commands.UploadFile;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Application.Features.Files.Queries.GetDownload;
using CloudNet.Application.Features.Files.Queries.ListByFolder;
using CloudNet.Application.Features.Files.Queries.ListDeleted;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting(RateLimitingPolicyNames.PerUser)]
[Route("api/v{version:apiVersion}/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorage _storage;

    public FilesController(IMediator mediator, IFileStorage storage)
    {
        _mediator = mediator;
        _storage = storage;
    }

    [HttpPost]
    public async Task<ActionResult<FileEntryDto>> Create([FromBody] CreateFileEntryRequest request, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var dto = new CreateFileEntryDto
        {
            OwnerId = ownerId,
            FolderId = request.FolderId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            StoragePath = request.StoragePath,
            Description = request.Description
        };

        var result = await _mediator.Send(new CreateFileEntryCommand(dto), ct);
        return Ok(result);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(FileUploadConstants.MaxUploadSizeBytes)]
    public async Task<ActionResult<FileEntryDto>> Upload([FromForm] UploadFileRequest request, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        if (request.File is null || request.File.Length == 0)
            return BadRequest("File is required.");

        await using var stream = request.File.OpenReadStream();

        var result = await _mediator.Send(
            new UploadFileCommand(
                ownerId,
                request.FolderId,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                stream,
                request.Description),
            ct);

        return Ok(result);
    }

    [HttpPatch("{fileId:guid}")]
    public async Task<ActionResult<FileEntryDto>> Update(
        [FromRoute] Guid fileId,
        [FromBody] UpdateFileEntryRequest request,
        CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var dto = new UpdateFileEntryDto
        {
            FileId = fileId,
            OwnerId = ownerId,
            FileName = request.FileName,
            Description = request.Description
        };

        var result = await _mediator.Send(new UpdateFileEntryCommand(dto), ct);
        return Ok(result);
    }

    [HttpPut("{fileId:guid}/content")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(FileUploadConstants.MaxUploadSizeBytes)]
    public async Task<ActionResult<FileEntryDto>> ReplaceContent(
        [FromRoute] Guid fileId,
        [FromForm] ReplaceFileContentRequest request,
        CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        if (request.File is null || request.File.Length == 0)
            return BadRequest("File is required.");

        await using var stream = request.File.OpenReadStream();

        var result = await _mediator.Send(
            new ReplaceFileContentCommand(
                ownerId,
                fileId,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                stream),
            ct);

        return Ok(result);
    }

    [HttpGet("{fileId:guid}/download")]
    public async Task<IActionResult> Download([FromRoute] Guid fileId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var file = await _mediator.Send(new GetFileDownloadQuery(ownerId, fileId), ct);
        var stream = await _storage.OpenReadAsync(file.StoragePath, ct);

        return File(stream, file.ContentType, file.FileName, enableRangeProcessing: true);
    }

    [HttpGet("by-folder/{folderId:guid}")]
    public async Task<ActionResult<IReadOnlyList<FileEntryDto>>> ListByFolder(
        [FromRoute] Guid folderId,
        CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new ListFilesByFolderQuery(ownerId, folderId), ct);
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<IReadOnlyList<FileEntryDto>>> ListDeleted(CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new ListDeletedFilesQuery(ownerId), ct);
        return Ok(result);
    }

    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> SoftDelete([FromRoute] Guid fileId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        await _mediator.Send(new SoftDeleteFileCommand(ownerId, fileId), ct);
        return NoContent();
    }

    [HttpDelete("{fileId:guid}/purge")]
    public async Task<IActionResult> Purge([FromRoute] Guid fileId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        await _mediator.Send(new PurgeFileCommand(ownerId, fileId), ct);
        return NoContent();
    }

    [HttpPost("{fileId:guid}/restore")]
    public async Task<IActionResult> Restore([FromRoute] Guid fileId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        await _mediator.Send(new RestoreFileCommand(ownerId, fileId), ct);
        return NoContent();
    }

    [HttpPost("{fileId:guid}/share-links")]
    public async Task<ActionResult<ShareLinkTokenDto>> CreateShareLink(
        [FromRoute] Guid fileId,
        [FromBody] CreateShareLinkRequest request,
        CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(
            new CreateShareLinkCommand(ownerId, fileId, request.ExpiresAt, request.MaxDownloads),
            ct);

        return Ok(result);
    }
}
