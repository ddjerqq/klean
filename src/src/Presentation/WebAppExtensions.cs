using Application;
using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Presentation.Hubs;

namespace Presentation;

public static class WebAppExtensions
{
    public static void ConfigureAssemblies(this ConfigureWebHostBuilder builder)
    {
        // assemblies
        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(ConfigurationBase).IsAssignableFrom(type))
            .Where(type => type is { IsInterface: false, IsAbstract: false })
            .Where(type => type.Name.StartsWith("configure", StringComparison.InvariantCultureIgnoreCase))
            .Select(type => (IHostingStartup)Activator.CreateInstance(type)!)
            .ToList()
            .ForEach(hostingStartup =>
            {
                var name = hostingStartup.GetType().Name.Replace("Configure", "");
                Console.WriteLine($@"[{DateTime.UtcNow:s}.000 INF] Configured {name}");
                hostingStartup.Configure(builder);
            });
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        if (dbContext.Database.GetPendingMigrations().Any())
            dbContext.Database.Migrate();
    }

    public static void UseDevelopmentMiddleware(this WebApplication app)
    {
        app.UseMigrationsEndPoint();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void UseProductionMiddleware(this WebApplication app)
    {
        app.UseHsts();
        app.UseIdempotency();
    }

    public static void UseAppMiddleware(this WebApplication app)
    {
        app.UseRouting();
        app.UseRequestLocalization();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        // compress and then cache static files
        app.UseResponseCompression();
        app.UseResponseCaching();
    }

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

    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapSwagger();

        app.MapHealthChecks("/api/v1/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            AllowCachingResponses = false,
        });

        app.MapControllers();
        app.MapDefaultControllerRoute();

        app.MapHub<EventHub>("/ws");
    }
}