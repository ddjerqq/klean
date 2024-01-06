using System.Security.Claims;
using System.Threading.RateLimiting;
using Application.Common.Interfaces;
using Infrastructure.BackgroundJobs;
using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.RateLimiting;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Infrastructure;

/// <summary>
/// The <see cref="ConfigureServices" /> class.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Adds the infrastructure services to the service collection.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddIdempotency();
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Adds the persistence services to the service collection.
    /// </summary>
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<EntitySaveChangesInterceptor>();
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<AppDbContext>(o =>
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is "Development")
            {
                Console.Error.WriteLine("Running in development mode!!!");
                o.EnableDetailedErrors();
                o.EnableSensitiveDataLogging();
            }

            // TODO change this so its only InMemory database
            if (TryLoadConnectionString(out var pgConnectionString))
            {
                // o.UseNpgsql(pgConnectionString);
            }
            else
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                // o.UseSqlite(connectionString);
            }
        });

        // delegate the IDbContext to the EmeraldDbContext;
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        return services;
    }

    /// <summary>
    /// Adds the infrastructure services to the service collection.
    /// </summary>
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddQuartz(config =>
        {
            var jobKey = new JobKey("ProcessOutboxMessagesJob");
            config
                .AddJob<ProcessOutboxMessagesBackgroundJob>(jobKey)
                .AddTrigger(trigger => trigger
                    .ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule
                        .WithInterval(TimeSpan.FromSeconds(10))
                        .RepeatForever()));
        });

        services.AddQuartzHostedService();

        return services;
    }

    /// <summary>
    /// Adds the rate limiting services to the service collection.
    /// </summary>
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var policies = RateLimitConstants.LoadRateLimitOptions(configuration)
            .ToList();

        var globalPolicy = policies
            .First(x => x.PolicyName == RateLimitConstants.GlobalPolicyName);

        services.AddRateLimiter(rateLimitOptions =>
        {
            rateLimitOptions.RejectionStatusCode = 429;

            rateLimitOptions.OnRejected = (ctx, _) =>
            {
                if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    ctx.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString("R");

                return ValueTask.CompletedTask;
            };

            rateLimitOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
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

                return RateLimitPartition.GetTokenBucketLimiter(key, _ => globalPolicy);
            });
        });

        return services;
    }

    private static bool TryLoadConnectionString(out string connectionString)
    {
        connectionString = string.Empty;

        var dbHost = GetEnv("POSTGRES_HOST", "localhost");
        var port = GetEnv("POSTGRES_PORT", "5432");
        var db = GetEnv("POSTGRES_DB", "postgres");
        var user = GetEnv("POSTGRES_USER", "postgres");

        var password = GetEnv("POSTGRES_PASSWORD");
        if (string.IsNullOrEmpty(password))
            return false;

        var inDevelopment = GetEnv("DOTNET_ENVIRONMENT") == "Development"
                            || GetEnv("ASPNETCORE_ENVIRONMENT") == "Development";

        connectionString =
            $"Host={dbHost};" +
            $"Port={port};" +
            $"Database={db};" +
            $"Username={user};" +
            $"Password={password};" +
            $"Include Error Detail={inDevelopment}";

        return true;

        string GetEnv(string key, string? @default = null)
        {
            return Environment.GetEnvironmentVariable(key) ?? @default!;
        }
    }
}
