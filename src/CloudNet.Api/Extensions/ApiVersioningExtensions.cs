using Asp.Versioning;

namespace CloudNet.Api.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddCloudNetApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Support: /api/v1/...  OR  headers/querystring if you want later
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader()
                );

                options.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // v1, v1.0 ...
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
