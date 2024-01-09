using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Common.Extensions;
using Infrastructure.Auth;
using Infrastructure.BackgroundJobs;
using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
        services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
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
