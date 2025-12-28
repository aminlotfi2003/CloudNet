using Asp.Versioning;
using CloudNet.Api.Abstractions.Contracts.Auth;
using CloudNet.Api.Abstractions.Extensions;
using CloudNet.Api.Abstractions.RateLimiting;
using CloudNet.Application.Features.Auth.Commands.ChangePassword;
using CloudNet.Application.Features.Auth.Commands.ForgotPassword;
using CloudNet.Application.Features.Auth.Commands.Login;
using CloudNet.Application.Features.Auth.Commands.Logout;
using CloudNet.Application.Features.Auth.Commands.Refresh;
using CloudNet.Application.Features.Auth.Commands.Register;
using CloudNet.Application.Features.Auth.Commands.ResetPassword;
using CloudNet.Application.Features.Auth.Queries.GetMyLoginHistory;
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
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
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
        var ipAddress = ResolveIpAddress();
        var userAgent = ResolveUserAgent();
        var result = await _mediator.Send(
            new LoginCommand(request.Identifier, request.Password, device, ipAddress, userAgent),
            ct);

        return Ok(AuthResponse.MapAuthResponse(result));
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await _mediator.Send(new ForgotPasswordCommand(request.Identifier), ct);
        return Ok();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        await _mediator.Send(
            new ResetPasswordCommand(request.Identifier, request.ResetToken, request.NewPassword),
            ct);
        return Ok();
    }

    [HttpPost("change-password")]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        await _mediator.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword), ct);
        return Ok();
    }

    [HttpGet("login-history")]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<LoginHistoryResponse>>> LoginHistory(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new GetMyLoginHistoryQuery(userId), ct);
        var response = result.Select(LoginHistoryResponse.Map).ToList();
        return Ok(response);
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
    [EnableRateLimiting(RateLimitingPolicyNames.AuthSensitive)]
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

    private string? ResolveUserAgent()
        => Request.Headers.UserAgent.ToString();

    private string? ResolveIpAddress()
        => HttpContext.Connection.RemoteIpAddress?.ToString();
}
