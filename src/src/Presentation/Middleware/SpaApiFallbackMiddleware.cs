#pragma warning disable CS1591
namespace Presentation.Middleware;

public sealed class SpaApiFallbackMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("API endpoint not found");
            return;
        }

        await next(context);
    }
}

public static class SpaApiFallbackMiddlewareExtensions
{
    public static IServiceCollection AddSpaApiFallbackMiddleware(this IServiceCollection services) =>
        services.AddScoped<SpaApiFallbackMiddleware>();

    public static IApplicationBuilder UseSpaApiFallbackMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<SpaApiFallbackMiddleware>();
}