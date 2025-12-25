using CloudNet.Api.Abstractions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CloudNet.Api.Abstractions.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddCloudNetExceptionHandling(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseCloudNetExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
