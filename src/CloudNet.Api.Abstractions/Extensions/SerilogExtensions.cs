using CloudNet.Api.Abstractions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Security.Claims;

namespace CloudNet.Api.Abstractions.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);

            var seqUrl = context.Configuration["Seq:ServerUrl"];
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                var apiKey = context.Configuration["Seq:ApiKey"];
                loggerConfiguration.WriteTo.Seq(
                    seqUrl,
                    apiKey: string.IsNullOrWhiteSpace(apiKey) ? null : apiKey
                );
            }
        });

        return builder;
    }

    public static IApplicationBuilder UseAppRequestLogging(this IApplicationBuilder app)
    {
        app.UseCorrelationId();

        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, elapsed, exception) =>
            {
                if (exception is not null || httpContext.Response.StatusCode >= 500)
                    return LogEventLevel.Error;

                if (httpContext.Response.StatusCode >= 400)
                    return LogEventLevel.Warning;

                var path = httpContext.Request.Path;
                if (path.StartsWithSegments("/health") || path.StartsWithSegments("/swagger"))
                    return LogEventLevel.Debug;

                return LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.ItemName, out var correlationId))
                {
                    diagnosticContext.Set("CorrelationId", correlationId!); // ToDo: Fix nullable correlationId
                }

                diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);

                var userId = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    diagnosticContext.Set("UserId", userId);
                }
            };
        });

        return app;
    }
}
