using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CloudNet.Api.Abstractions.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseCloudNetExceptionHandling();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCloudNetRateLimiting();

        app.MapControllers();

        // Swagger after app build
        app.UseCloudNetSwagger();

        return app;
    }
}
