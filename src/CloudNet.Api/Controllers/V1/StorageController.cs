using Asp.Versioning;
using CloudNet.Api.Abstractions.Extensions;
using CloudNet.Api.Abstractions.RateLimiting;
using CloudNet.Application.Features.Storage.Dtos;
using CloudNet.Application.Features.Storage.Queries.GetMyStorageSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting(RateLimitingPolicyNames.PerUser)]
[Route("api/v{version:apiVersion}/storage")]
public sealed class StorageController : ControllerBase
{
    private readonly IMediator _mediator;

    public StorageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<StorageSummaryDto>> GetSummary(CancellationToken ct)
    {
        var ownerId = User.GetUserId();
        if (ownerId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new GetMyStorageSummaryQuery(ownerId), ct);
        return Ok(result);
    }
}
