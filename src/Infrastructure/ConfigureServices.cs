using System.Security.Claims;
using System.Threading.RateLimiting;
using Application.Common.Interfaces;
using Domain.Common.Extensions;
using Infrastructure.Auth;
using Infrastructure.BackgroundJobs;
using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.RateLimiting;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<EntitySaveChangesInterceptor>();
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<AppDbContext>(o =>
        {
            if ("ASPNETCORE_ENVIRONMENT".FromEnv() is "Development")
            {
                o.EnableDetailedErrors();
                o.EnableSensitiveDataLogging();
            }

            o.UseInMemoryDatabase("app");
        });

        // delegate the IDbContext to the AppDbContext;
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

    /// <summary>
    /// Adds the authentication and authorization services to the service collection
    /// </summary>
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.Events = Jwt.Events;
                options.Audience = Jwt.ClaimsAudience;
                options.ClaimsIssuer = Jwt.ClaimsIssuer;
                options.TokenValidationParameters = Jwt.TokenValidationParameters;
            });

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy("default", policy => policy.RequireAuthenticatedUser())
            .AddPolicy("is_elon", policy => policy.RequireClaim(ClaimTypes.NameIdentifier, "elon"));

        return services;
    }
}
