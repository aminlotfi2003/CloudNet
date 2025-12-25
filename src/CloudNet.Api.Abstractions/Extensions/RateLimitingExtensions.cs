using CloudNet.Api.Abstractions.Options;
using CloudNet.Api.Abstractions.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading.RateLimiting;

namespace CloudNet.Api.Abstractions.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCloudNetRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName));

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                var httpContext = context.HttpContext;
                var logger = httpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("RateLimiting");

                var policyName = ResolvePolicyName(httpContext);

                logger.LogWarning(
                    "Rate limit rejected request for {Path} using policy {PolicyName}.",
                    httpContext.Request.Path,
                    policyName);

                var rateLimitingOptions = httpContext.RequestServices
                    .GetRequiredService<IOptions<RateLimitingOptions>>()
                    .Value;

                var retryAfterSeconds = context.Lease.TryGetMetadata(
                        MetadataName.RetryAfter,
                        out var retryAfter)
                    ? Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))
                    : rateLimitingOptions.DefaultRetryAfterSeconds;

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                httpContext.Response.ContentType = "application/problem+json";
                httpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too Many Requests",
                    Detail = "Too many requests. Please try again later.",
                    Instance = httpContext.Request.Path
                };

                problemDetails.Extensions["policy"] = policyName;

                if (TryGetSafePartitionKey(httpContext, policyName, out var partitionKey))
                {
                    problemDetails.Extensions["partitionKey"] = partitionKey;
                }

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };

            options.AddPolicy(RateLimitingPolicyNames.PerUser, httpContext =>
            {
                var settings = httpContext.RequestServices
                    .GetRequiredService<IOptions<RateLimitingOptions>>()
                    .Value;

                if (httpContext.User.Identity?.IsAuthenticated == true
                    && httpContext.User.TryGetUserId(out var userId))
                {
                    var partitionKey = $"user:{userId}";
                    httpContext.Items[RateLimitingConstants.PartitionKeyItemName] = partitionKey;

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = settings.PerUser.PermitLimit,
                            Window = TimeSpan.FromSeconds(settings.PerUser.WindowSeconds),
                            QueueLimit = settings.PerUser.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                }

                // If the claim is missing/invalid, fall back to a stricter per-IP limiter
                // to avoid a shared Guid.Empty partition while still throttling suspicious traffic.
                var fallbackKey = GetRemoteIpPartitionKey(httpContext, "invalid-user");
                httpContext.Items[RateLimitingConstants.PartitionKeyItemName] = fallbackKey;

                return RateLimitPartition.GetFixedWindowLimiter(
                    fallbackKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.InvalidUser.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.InvalidUser.WindowSeconds),
                        QueueLimit = settings.InvalidUser.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });

            options.AddPolicy(RateLimitingPolicyNames.AuthSensitive, httpContext =>
            {
                var settings = httpContext.RequestServices
                    .GetRequiredService<IOptions<RateLimitingOptions>>()
                    .Value;

                var partitionKey = GetRemoteIpPartitionKey(httpContext, "auth");
                httpContext.Items[RateLimitingConstants.PartitionKeyItemName] = partitionKey;

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.AuthSensitive.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.AuthSensitive.WindowSeconds),
                        QueueLimit = settings.AuthSensitive.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCloudNetRateLimiting(this IApplicationBuilder app)
        => app.UseRateLimiter();

    private static string GetRemoteIpPartitionKey(HttpContext httpContext, string prefix)
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"{prefix}:{ip}";
    }

    private static bool TryGetSafePartitionKey(
        HttpContext httpContext,
        string? policyName,
        out string partitionKey)
    {
        partitionKey = string.Empty;

        if (policyName != RateLimitingPolicyNames.PerUser)
        {
            return false;
        }

        if (httpContext.Items.TryGetValue(RateLimitingConstants.PartitionKeyItemName, out var key)
            && key is string keyValue
            && keyValue.StartsWith("user:", StringComparison.Ordinal))
        {
            partitionKey = keyValue;
            return true;
        }

        return false;
    }

    private static string ResolvePolicyName(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var attribute = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();
        return string.IsNullOrWhiteSpace(attribute?.PolicyName) ? "unknown" : attribute.PolicyName;
    }
}
