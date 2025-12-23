using Asp.Versioning;
using CloudNet.Application.Features.Files.Commands.CreateFile;
using CloudNet.Application.Features.Files.Commands.RestoreFile;
using CloudNet.Application.Features.Files.Commands.SoftDeleteFile;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Application.Features.Files.Queries.ListByFolder;
using CloudNet.Application.Features.Files.Queries.ListDeleted;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/files")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Create metadata (Upload physical file later via separate endpoint)
    [HttpPost]
    public async Task<ActionResult<FileEntryDto>> Create([FromBody] CreateFileEntryDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateFileEntryCommand(dto), ct);
        return Ok(result);
    }

    [HttpGet("by-folder/{folderId:guid}")]
    public async Task<ActionResult<IReadOnlyList<FileEntryDto>>> ListByFolder(
        [FromRoute] Guid folderId,
        [FromQuery] Guid ownerId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ListFilesByFolderQuery(ownerId, folderId), ct);
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<IReadOnlyList<FileEntryDto>>> ListDeleted([FromQuery] Guid ownerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListDeletedFilesQuery(ownerId), ct);
        return Ok(result);
    }

    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> SoftDelete([FromRoute] Guid fileId, [FromQuery] Guid ownerId, CancellationToken ct)
    {
        await _mediator.Send(new SoftDeleteFileCommand(ownerId, fileId), ct);
        return NoContent();
    }

    [HttpPost("{fileId:guid}/restore")]
    public async Task<IActionResult> Restore([FromRoute] Guid fileId, [FromQuery] Guid ownerId, CancellationToken ct)
    {
        await _mediator.Send(new RestoreFileCommand(ownerId, fileId), ct);
        return NoContent();
    }
}
