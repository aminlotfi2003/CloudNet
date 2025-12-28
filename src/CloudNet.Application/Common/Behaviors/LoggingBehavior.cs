using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CloudNet.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly string[] IdentifierPropertyNames =
    [
        "OwnerId",
        "UserId",
        "FileId",
        "FolderId",
        "ShareLinkId"
    ];

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var identifiers = ExtractIdentifiers(request);

        if (identifiers.Count > 0)
        {
            _logger.LogInformation("Handling {RequestName} {@Identifiers}", requestName, identifiers);
        }
        else
        {
            _logger.LogInformation("Handling {RequestName}", requestName);
        }

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        if (elapsedMs > 500)
        {
            _logger.LogWarning(
                "Handled {RequestName} in {ElapsedMs} ms {@Identifiers}",
                requestName,
                elapsedMs,
                identifiers.Count > 0 ? identifiers : null);
        }
        else
        {
            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs} ms {@Identifiers}",
                requestName,
                elapsedMs,
                identifiers.Count > 0 ? identifiers : null);
        }

        return response;
    }

    private static Dictionary<string, object?> ExtractIdentifiers(TRequest request)
    {
        var identifiers = new Dictionary<string, object?>();
        var requestType = request.GetType();

        foreach (var name in IdentifierPropertyNames)
        {
            var property = requestType.GetProperty(name);
            if (property is null)
                continue;

            var value = property.GetValue(request);
            if (value is null)
                continue;

            identifiers[name] = value;
        }

        return identifiers;
    }
}
