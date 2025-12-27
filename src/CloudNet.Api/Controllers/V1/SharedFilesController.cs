using Asp.Versioning;
using CloudNet.Api.Abstractions.RateLimiting;
using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Application.Features.Files.Queries.GetSharedDownload;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingPolicyNames.ShareDownload)]
[Route("api/v{version:apiVersion}/files/share")]
public sealed class SharedFilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorage _storage;

    public SharedFilesController(IMediator mediator, IFileStorage storage)
    {
        _mediator = mediator;
        _storage = storage;
    }

    [HttpGet("{token}/download")]
    public async Task<IActionResult> DownloadShared([FromRoute] string token, CancellationToken ct)
    {
        var file = await _mediator.Send(new GetSharedFileDownloadQuery(token), ct);
        var stream = await _storage.OpenReadAsync(file.StoragePath, ct);

        return File(stream, file.ContentType, file.FileName, enableRangeProcessing: true);
    }
}
