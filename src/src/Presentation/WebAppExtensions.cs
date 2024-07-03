using Infrastructure.Idempotency;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Presentation.Components;
using Serilog;
using WebAPI;
using WebAPI.Hubs;

namespace Presentation;

/// <summary>
/// Web application extensions
/// </summary>
public static class WebAppExt
{
    /// <summary>
    /// Apply any pending migrations to the database if any.
    /// </summary>
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            var migrations = dbContext.Database.GetPendingMigrations();
            Log.Information("Applying migrations: {@Migrations}", string.Join(", ", migrations));
            dbContext.Database.Migrate();
            Log.Information("All migrations applied");
        }

        dbContext.SaveChanges();
    }

    /// <summary>
    /// Use middleware specifically for Development environments
    /// </summary>
    public static void UseDevelopmentMiddleware(this WebApplication app)
    {
        app.UseMigrationsEndPoint();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /// <summary>
    /// Use middleware specifically for Production environments
    /// </summary>
    public static void UseProductionMiddleware(this WebApplication app)
    {
        app.UseHsts();
        app.UseIdempotency();

        // compress and then cache static files
        app.UseResponseCompression();
        app.UseResponseCaching();
    }


    /// <summary>
    /// Use general web app middleware
    /// </summary>
    public static void UseGeneralMiddleware(this WebApplication app)
    {
        app.UseRouting();
        app.UseRequestLocalization();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.UseStaticFiles();
    }

    /// <summary>
    /// Add some security related headers to your web app.
    /// </summary>
    public static void UseCustomHeaderMiddleware(this WebApplication app)
    {
        app.Use((ctx, next) =>
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            ctx.Response.Headers.Append("X-Frame-Options", "DENY");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            ctx.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            return next();
        });
    }

    /// <summary>
    /// Use the global exception handler
    /// </summary>
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }

    /// <summary>
    /// Map the endpoints and websockets
    /// </summary>
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/foo", () => "foo bar baz");

        app.MapSwagger();

        app.MapHealthChecks("/api/v1/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            AllowCachingResponses = false,
        });

        app.MapControllers();
        app.MapDefaultControllerRoute();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapHub<EventHub>("/ws");
    }
}