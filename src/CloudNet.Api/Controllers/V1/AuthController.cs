using Asp.Versioning;
using CloudNet.Application.Features.Auth.Commands.Login;
using CloudNet.Application.Features.Auth.Commands.Logout;
using CloudNet.Application.Features.Auth.Commands.Refresh;
using CloudNet.Application.Features.Auth.Commands.Register;
using CloudNet.Application.Features.Auth.Dtos;
using CloudNet.Infrastructure.Identity.Auth;
using CloudNet.Infrastructure.Identity.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CloudNet.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtOptions _jwtOptions;

    public AuthController(IMediator mediator, IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
    {
        await _mediator.Send(new RegisterCommand(dto), ct);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        var tokens = await _mediator.Send(new LoginCommand(dto), ct);

        Response.Cookies.Append(AuthCookies.AccessToken, tokens.AccessToken,
            CookieOptionsFactory.AccessToken(_jwtOptions.AccessTokenMinutes));

        Response.Cookies.Append(AuthCookies.RefreshToken, tokens.RefreshToken,
            CookieOptionsFactory.RefreshToken(_jwtOptions.RefreshTokenDays));

        return Ok(new { accessToken = tokens.AccessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshRaw = Request.Cookies[AuthCookies.RefreshToken];
        if (string.IsNullOrWhiteSpace(refreshRaw))
            return Unauthorized();

        var tokens = await _mediator.Send(
            new RefreshCommand(new RefreshRequestDto { RefreshToken = refreshRaw }),
            ct);

        Response.Cookies.Append(AuthCookies.AccessToken, tokens.AccessToken,
            CookieOptionsFactory.AccessToken(_jwtOptions.AccessTokenMinutes));

        Response.Cookies.Append(AuthCookies.RefreshToken, tokens.RefreshToken,
            CookieOptionsFactory.RefreshToken(_jwtOptions.RefreshTokenDays));

        return Ok(new { accessToken = tokens.AccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshRaw = Request.Cookies[AuthCookies.RefreshToken] ?? string.Empty;

        await _mediator.Send(new LogoutCommand(refreshRaw), ct);

        Response.Cookies.Delete(AuthCookies.AccessToken);
        Response.Cookies.Delete(AuthCookies.RefreshToken);

        return NoContent();
    }
}
