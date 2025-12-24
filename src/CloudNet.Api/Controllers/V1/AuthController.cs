using Asp.Versioning;
using CloudNet.Api.Contracts.Auth;
using CloudNet.Api.Security;
using CloudNet.Application.Features.Auth.Commands.Login;
using CloudNet.Application.Features.Auth.Commands.Logout;
using CloudNet.Application.Features.Auth.Commands.Refresh;
using CloudNet.Application.Features.Auth.Commands.Register;
using CloudNet.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
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

        return StatusCode(StatusCodes.Status201Created, MapAuthResponse(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var device = ResolveDevice();
        var result = await _mediator.Send(
            new LoginCommand(request.Identifier, request.Password, device),
            ct);

        return Ok(MapAuthResponse(result));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthTokensResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var device = ResolveDevice();
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken, device), ct);
        return Ok(MapTokensResponse(result));
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
        => Request.Headers["User-Agent"].ToString();

    private static AuthResponse MapAuthResponse(AuthResponseDto dto)
        => new()
        {
            User = new AuthUserResponse
            {
                Id = dto.User.Id,
                Email = dto.User.Email,
                UserName = dto.User.UserName
            },
            Tokens = MapTokensResponse(dto.Tokens)
        };

    private static AuthTokensResponse MapTokensResponse(AuthTokensDto dto)
        => new()
        {
            AccessToken = dto.AccessToken,
            AccessTokenExpiresAt = dto.AccessTokenExpiresAt,
            RefreshToken = dto.RefreshToken,
            RefreshTokenExpiresAt = dto.RefreshTokenExpiresAt
        };
}
