using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Idempotency;

/// <summary>
/// The <see cref="IdempotencyMiddleware" /> class.
/// Ensures that requests with the same idempotency key are not processed more than once.
/// </summary>
public sealed class IdempotencyMiddleware(IIdempotencyService idempotencyService) : IMiddleware
{
    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var attributes = endpoint?.Metadata.OfType<RequireIdempotencyAttribute>();
        var hasRequireIdempotency = attributes?.Any() ?? false;

        if (!hasRequireIdempotency)
        {
            await next(context);
            return;
        }

        var idempotencyKey = context.Request.Headers["X-Idempotency-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out var key))
        {
            context.Response.Headers.Clear();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsync("Invalid / missing idempotency key");

            return;
        }

        if (idempotencyService.ContainsKey(key))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            await context.Response.WriteAsync("Conflicting idempotency key");

            return;
        }

        idempotencyService.AddKey(key);

        await next(context);
    }
}

/// <summary>
/// The <see cref="IdempotencyMiddlewareExtensions" /> class.
/// </summary>
public static class IdempotencyMiddlewareExtensions
{
    /// <summary>
    /// Adds the idempotency services to the service collection.
    /// </summary>
    public static IServiceCollection AddIdempotency(this IServiceCollection services)
    {
        services.AddScoped<IdempotencyMiddleware>();
        services.AddSingleton<IIdempotencyService, IdempotencyService>();

        return services;
    }

    /// <summary>
    /// Adds the idempotency middleware to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IdempotencyMiddleware>();
    }
}