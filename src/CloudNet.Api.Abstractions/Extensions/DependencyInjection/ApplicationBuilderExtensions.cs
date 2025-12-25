using Microsoft.AspNetCore.Builder;

namespace CloudNet.Api.Abstractions.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseCloudNetExceptionHandling();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Swagger after app build
        app.UseCloudNetSwagger();

        return app;
    }
}
