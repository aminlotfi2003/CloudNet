using Asp.Versioning;
using CloudNet.Api.Abstractions.Contracts.Files;
using CloudNet.Api.Abstractions.Extensions;
using CloudNet.Application.Features.Files.Commands.CreateFile;
using CloudNet.Application.Features.Files.Commands.RestoreFile;
using CloudNet.Application.Features.Files.Commands.SoftDeleteFile;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Application.Features.Files.Queries.ListByFolder;
using CloudNet.Application.Features.Files.Queries.ListDeleted;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
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

    [HttpPost("{fileId:guid}/restore")]
    public async Task<IActionResult> Restore([FromRoute] Guid fileId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        await _mediator.Send(new RestoreFileCommand(ownerId, fileId), ct);
        return NoContent();
    }
}
