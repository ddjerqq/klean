using System.Threading.RateLimiting;
using Application;
using Application.Services;
using Domain.Aggregates;
using Domain.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config;

public sealed class ConfigureRateLimiting : ConfigurationBase
{
    private static readonly PartitionedRateLimiter<HttpContext> GlobalRateLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context =>
        {
            string key;

            if (context.Items.TryGetValue(nameof(User), out var value) && value is User user)
                key = user.Id.Value.ToString();
            else
                key = context.Connection.RemoteIpAddress?.ToString() ?? context.Connection.Id;

            return RateLimitPartition.GetTokenBucketLimiter(key, _ => GlobalPolicy);
        });

    private static TokenBucketRateLimiterOptions GlobalPolicy => new()
    {
        AutoReplenishment = true,
        QueueLimit = int.Parse("RATE_LIMIT__QUEUE_LIMIT".FromEnv("50")),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        ReplenishmentPeriod =
            TimeSpan.FromSeconds(int.Parse("RATE_LIMIT__REPLENISHMENT_PERIOD_SECONDS".FromEnv("1"))),
        TokenLimit = int.Parse("RATE_LIMIT__TOKEN_LIMIT".FromEnv("30")),
        TokensPerPeriod = int.Parse("RATE_LIMIT__TOKENS_PER_PERIOD".FromEnv("20")),
    };

    private static ValueTask OnRejectedAsync(OnRejectedContext ctx, CancellationToken ct)
    {
        var dateTimeProvider = ctx.HttpContext.RequestServices.GetRequiredService<IDateTimeProvider>();

        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            var retryAfterDateRelative = dateTimeProvider.UtcNow.Add(retryAfter);
            ctx.HttpContext.Response.Headers.RetryAfter = retryAfterDateRelative.ToString("R");
        }

        return ValueTask.CompletedTask;
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = OnRejectedAsync;
            options.GlobalLimiter = GlobalRateLimiter;
        });
    }
}