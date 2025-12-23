using Asp.Versioning;
using CloudNet.Api.Contracts.Folders;
using CloudNet.Api.Security;
using CloudNet.Application.Features.Folders.Commands.CreateFolder;
using CloudNet.Application.Features.Folders.Commands.RestoreFolder;
using CloudNet.Application.Features.Folders.Commands.SoftDeleteFolder;
using CloudNet.Application.Features.Folders.Dtos;
using CloudNet.Application.Features.Folders.Queries.ListChildren;
using CloudNet.Application.Features.Folders.Queries.ListDeleted;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/folders")]
public sealed class FoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<FolderDto>> Create([FromBody] CreateFolderRequest request, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        // API contract doesn't include OwnerId; we set it from claim
        var dto = new CreateFolderDto
        {
            OwnerId = ownerId,
            ParentId = request.ParentId,
            Name = request.Name
        };

        var result = await _mediator.Send(new CreateFolderCommand(dto), ct);
        return Ok(result);
    }

    [HttpGet("children")]
    public async Task<ActionResult<IReadOnlyList<FolderDto>>> ListChildren(
        [FromQuery] Guid? parentId,
        CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new ListFolderChildrenQuery(ownerId, parentId), ct);
        return Ok(result);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<IReadOnlyList<FolderDto>>> ListDeleted(CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new ListDeletedFoldersQuery(ownerId), ct);
        return Ok(result);
    }

    [HttpDelete("{folderId:guid}")]
    public async Task<IActionResult> SoftDelete([FromRoute] Guid folderId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        await _mediator.Send(new SoftDeleteFolderCommand(ownerId, folderId), ct);
        return NoContent();
    }

    [HttpPost("{folderId:guid}/restore")]
    public async Task<IActionResult> Restore([FromRoute] Guid folderId, CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        await _mediator.Send(new RestoreFolderCommand(ownerId, folderId), ct);
        return NoContent();
    }
}
