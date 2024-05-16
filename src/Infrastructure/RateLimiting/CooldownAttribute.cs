using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.RateLimiting;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CooldownAttribute(int rate, double perSeconds) : ActionFilterAttribute
{
    public TimeSpan Per { get; set; } = TimeSpan.FromSeconds(perSeconds);

    [SuppressMessage("Usage", "ASP0019", Justification = "This is a filter, not a controller action")]
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext
            .RequestServices
            .GetRequiredService<IMemoryCache>();

        var dateTimeProvider = context.HttpContext
            .RequestServices
            .GetRequiredService<IDateTimeProvider>();

        var uri = context.HttpContext.Request.Path.Value;
        var key = GetDefaultKey(context.HttpContext);
        var cacheKey = $"cooldown:{uri}:{key}";

        RateLimitCacheEntry? cacheEntry = await cache
            .GetOrCreateAsync(cacheKey, entry =>
            {
                entry.SetAbsoluteExpiration(Per);
                return Task.FromResult(new RateLimitCacheEntry(rate, Per));
            });

        if (cacheEntry?.TryAcquire(dateTimeProvider, out var retryAfter) ?? true)
        {
            await next();
            return;
        }

        context.Result = new StatusCodeResult(429);
        context.HttpContext.Response.Headers.Add("Retry-After", retryAfter.ToString("R"));
    }

    private static string GetDefaultKey(HttpContext context)
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? context.Connection.Id;

        if (context.User is { Identity.IsAuthenticated: true, Claims: var claims })
        {
            var id = claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                .Value;

            if (!string.IsNullOrEmpty(id))
                key = id;
        }

        return key;
    }
}