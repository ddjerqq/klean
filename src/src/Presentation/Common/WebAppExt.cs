#pragma warning disable ASP0014
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Serilog;

namespace Presentation.Common;

/// <summary>
///     Web application extensions
/// </summary>
public static class WebAppExt
{
    /// <summary>
    ///     Apply any pending migrations to the database if any.
    /// </summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            var migrations = await dbContext.Database.GetPendingMigrationsAsync();
            Log.Information("Applying migrations: {Migrations}", string.Join(", ", migrations));
            await dbContext.Database.MigrateAsync();
        }

        Log.Information("All migrations applied");

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Use general web app middleware
    /// </summary>
    public static void UseApplicationMiddleware(this WebApplication app)
    {
        app.UseGlobalExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        if (app.Environment.IsProduction())
        {
            // compress and then cache static files only in production
            app.UseResponseCompression();
            app.UseResponseCaching();
        }

        app.UseStaticFiles();

        // app.UseRouting();
        app.UseRateLimiter();
        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseCustomHeaderMiddleware();

        app.MapAppHealthChecks();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapFallback(ctx =>
        {
            // catch requests that dont really exist?
            ctx.Response.Redirect("404");
            return Task.CompletedTask;
        });
    }

    private static void UseCustomHeaderMiddleware(this WebApplication app)
    {
        app.Use((ctx, next) =>
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            ctx.Response.Headers.Append("X-Frame-Options", "DENY");

            // // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // ctx.Response.Headers.Append("X-Made-By", "tenxdeveloper");

            return next();
        });
    }

    private static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }

    private static void MapAppHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/api/v1/health", new HealthCheckOptions
        {
            // if the predicate is null, then all checks are included
            // Predicate = _ => true,
            AllowCachingResponses = false,
        });
    }
}