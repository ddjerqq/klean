using System.ComponentModel;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Application.Abstractions;
using Infrastructure;
using Infrastructure.Idempotency;
using Infrastructure.RateLimiting;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureInfrastructure))]

namespace Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureInfrastructure : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddScoped<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddIdempotency();
            services.AddMemoryCache();
        });

        // add rate limiting
        builder.ConfigureServices(services =>
        {

        });
    }

    public static IServiceCollection AddRateLimiting(IServiceCollection services, IConfiguration configuration)
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
}
