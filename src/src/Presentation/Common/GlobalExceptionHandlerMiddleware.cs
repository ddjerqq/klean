﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Common;

/// <inheritdoc />
public sealed class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger) : IMiddleware
{
    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (ValidationException ex)
        {
            await TryHandleAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            await TryHandleAsync(httpContext, ex);
        }
    }

    private static bool IsDev(HttpContext httpContext)
    {
        return httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
    }

    private async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception)
    {
        logger.LogError(exception, "An unhandled exception occurred. {Message} {TraceId}", exception.Message, httpContext.TraceIdentifier);

        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.com/500",
            Title = "An error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = IsDev(httpContext) ? exception.Message : "Please come back later, we are working on fixing the issues!",
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["handler"] = "validation",
            },
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails);

        return true;
    }

    private async ValueTask<bool> TryHandleAsync(HttpContext httpContext, ValidationException exception)
    {
        logger.LogError(exception, "A validation exception occurred. {Message} {TraceId}", exception.Message, httpContext.TraceIdentifier);

        var errors = exception
            .Errors
            .ToDictionary(
                e => e?.ErrorCode ?? e?.ErrorMessage ?? "unspecified",
                e => new[] { e?.ErrorMessage });

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Type = "https://httpstatuses.com/400",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["handler"] = "global",
            },
        };

        if (errors.Count != 0 && IsDev(httpContext))
            problemDetails.Extensions["errors"] = errors;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails);

        return true;
    }
}