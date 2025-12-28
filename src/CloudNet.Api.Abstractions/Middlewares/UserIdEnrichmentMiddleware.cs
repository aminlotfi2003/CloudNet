using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CloudNet.Api.Abstractions.Middlewares;

public sealed class UserIdEnrichmentMiddleware : IMiddleware
{
    private readonly ILogger<UserIdEnrichmentMiddleware> _logger;

    public UserIdEnrichmentMiddleware(ILogger<UserIdEnrichmentMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                ["UserId"] = userId
            }))
            {
                await next(context);
                return;
            }
        }

        await next(context);
    }
}
