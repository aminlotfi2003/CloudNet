using CloudNet.Application;
using CloudNet.Infrastructure.Identity;
using CloudNet.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudNet.Api.Abstractions.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddCloudNetExceptionHandling();
        services.AddCloudNetRateLimiting(configuration);

        // Application
        services.AddApplication();

        // Persistence
        services.AddPersistence(configuration);

        // Identity
        services.AddInfrastructureIdentity(configuration);

        // Versioning + Swagger
        services.AddCloudNetApiVersioning();
        services.AddCloudNetSwagger();

        return services;
    }
}
