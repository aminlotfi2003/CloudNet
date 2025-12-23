using CloudNet.Api.Middlewares;

namespace CloudNet.Api.Extensions;

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
