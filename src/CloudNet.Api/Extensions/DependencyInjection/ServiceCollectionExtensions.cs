using CloudNet.Application;
using CloudNet.Infrastructure.Persistence;

namespace CloudNet.Api.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddCloudNetExceptionHandling();

        // Application
        services.AddApplication();

        // Persistence
        services.AddPersistence(configuration);

        // Versioning + Swagger
        services.AddCloudNetApiVersioning();
        services.AddCloudNetSwagger();

        return services;
    }
}
