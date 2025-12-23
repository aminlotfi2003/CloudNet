using Asp.Versioning;
using CloudNet.Application.Features.Folders.Commands.CreateFolder;
using CloudNet.Application.Features.Folders.Commands.RestoreFolder;
using CloudNet.Application.Features.Folders.Commands.SoftDeleteFolder;
using CloudNet.Application.Features.Folders.Dtos;
using CloudNet.Application.Features.Folders.Queries.ListChildren;
using CloudNet.Application.Features.Folders.Queries.ListDeleted;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/folders")]
public class FoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<FolderDto>> Create([FromBody] CreateFolderDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateFolderCommand(dto), ct);

        return CreatedAtAction(nameof(ListChildren), new
        {
            version = "1.0",
            ownerId = result.OwnerId,
            parentId = result.ParentId
        }, result);
    }

    [HttpGet("children")]
    public async Task<ActionResult<IReadOnlyList<FolderDto>>> ListChildren(
        [FromQuery] Guid ownerId,
        [FromQuery] Guid? parentId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ListFolderChildrenQuery(ownerId, parentId), ct);
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<IReadOnlyList<FolderDto>>> ListDeleted([FromQuery] Guid ownerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListDeletedFoldersQuery(ownerId), ct);
        return Ok(result);
    }

    [HttpDelete("{folderId:guid}")]
    public async Task<IActionResult> SoftDelete([FromRoute] Guid folderId, [FromQuery] Guid ownerId, CancellationToken ct)
    {
        await _mediator.Send(new SoftDeleteFolderCommand(ownerId, folderId), ct);
        return NoContent();
    }

    [HttpPost("{folderId:guid}/restore")]
    public async Task<IActionResult> Restore([FromRoute] Guid folderId, [FromQuery] Guid ownerId, CancellationToken ct)
    {
        await _mediator.Send(new RestoreFolderCommand(ownerId, folderId), ct);
        return NoContent();
    }
}
