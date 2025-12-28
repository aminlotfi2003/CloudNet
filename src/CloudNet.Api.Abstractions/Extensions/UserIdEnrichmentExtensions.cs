using CloudNet.Api.Abstractions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CloudNet.Api.Abstractions.Extensions;

public static class UserIdEnrichmentExtensions
{
    public static IServiceCollection AddCloudNetUserIdEnrichment(this IServiceCollection services)
    {
        services.AddTransient<UserIdEnrichmentMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseCloudNetUserIdEnrichment(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserIdEnrichmentMiddleware>();
    }
}
