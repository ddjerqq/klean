using System.Threading.RateLimiting;

namespace Infrastructure.RateLimiting;


/// <summary>
/// The <see cref="RateLimitOptions" /> class.
/// </summary>
/// <param name="PolicyName">The name of the policy</param>
/// <param name="ReplenishmentPeriod">How often the tokens are replenished</param>
/// <param name="QueueLimit">
/// Number of maximum tokens to be added to the queue, more items than this
/// will overflow the queue
/// </param>
/// <param name="TokenLimit">Number of maximum tokens</param>
/// <param name="TokensPerPeriod">
/// Number of tokens to be added to the queue per replenishment period
/// </param>
/// <param name="AutoReplenishment">
/// whether or not the tokens should be automatically replenished
/// </param>
public sealed record RateLimitOptions(
    string PolicyName,
    int ReplenishmentPeriod,
    int QueueLimit,
    int TokenLimit,
    int TokensPerPeriod,
    bool AutoReplenishment)
{
    /// <summary>
    /// The default rate limit options to use if no configuration is found.
    /// </summary>
    public static readonly RateLimitOptions Default = new("global", 1, 20, 100, 10, true);

    /// <summary>
    /// Converts the <see cref="RateLimitOptions" /> to a <see cref="TokenBucketRateLimiterOptions" />.
    /// </summary>
    public static implicit operator TokenBucketRateLimiterOptions(RateLimitOptions options)
    {
        return new TokenBucketRateLimiterOptions
        {
            AutoReplenishment = options.AutoReplenishment,
            QueueLimit = options.QueueLimit,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            ReplenishmentPeriod = TimeSpan.FromSeconds(options.ReplenishmentPeriod),
            TokenLimit = options.TokenLimit,
            TokensPerPeriod = options.TokensPerPeriod,
        };
    }
}