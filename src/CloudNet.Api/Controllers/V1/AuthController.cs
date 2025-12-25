using Asp.Versioning;
using CloudNet.Api.Abstractions.Contracts.Auth;
using CloudNet.Api.Abstractions.Extensions;
using CloudNet.Api.Abstractions.RateLimiting;
using CloudNet.Application.Features.Auth.Commands.Login;
using CloudNet.Application.Features.Auth.Commands.Logout;
using CloudNet.Application.Features.Auth.Commands.Refresh;
using CloudNet.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[EnableRateLimiting("PerUserFixedWindow")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var device = ResolveDevice();
        var result = await _mediator.Send(
            new RegisterCommand(request.Email, request.UserName, request.Password, request.ConfirmPassword, device),
            ct);

        return StatusCode(StatusCodes.Status201Created, AuthResponse.MapAuthResponse(result));
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var device = ResolveDevice();
        var result = await _mediator.Send(
            new LoginCommand(request.Identifier, request.Password, device),
            ct);

        return Ok(AuthResponse.MapAuthResponse(result));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
    [AllowAnonymous]
    public async Task<ActionResult<AuthTokensResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var device = ResolveDevice();
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken, device), ct);
        return Ok(AuthTokensResponse.MapTokensResponse(result));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        await _mediator.Send(new LogoutCommand(userId, request.RefreshToken), ct);
        return Ok();
    }

    private string? ResolveDevice()
        => Request.Headers.UserAgent.ToString();
}
