using CloudNet.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CloudNet.Api.Abstractions.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(IHostEnvironment env, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation failed"),
            NotFoundException => (HttpStatusCode.NotFound, "Not found"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            _ => (HttpStatusCode.InternalServerError, "Server error")
        };

        LogException(context, ex, status);

        var problem = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = _env.IsDevelopment() ? ex.Message : null,
            Instance = context.Request.Path
        };

        // Validation errors -> extensions["errors"]
        if (ex is ValidationException vex)
        {
            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            problem.Extensions["errors"] = errors;
        }

        // TraceId helpful for debugging
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsJsonAsync(problem);
    }

    private void LogException(HttpContext context, Exception ex, HttpStatusCode status)
    {
        var logLevel = ex switch
        {
            ValidationException => LogLevel.Information,
            NotFoundException => LogLevel.Information,
            ConflictException => LogLevel.Information,
            UnauthorizedException => LogLevel.Warning,
            ForbiddenException => LogLevel.Warning,
            _ => status == HttpStatusCode.InternalServerError ? LogLevel.Error : LogLevel.Warning
        };

        _logger.Log(logLevel, ex, "Request failed with {StatusCode} {Title}", (int)status, status);
    }
}
