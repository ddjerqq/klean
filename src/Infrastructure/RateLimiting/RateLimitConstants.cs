using Microsoft.Extensions.Configuration;

namespace Infrastructure.RateLimiting;

/// <summary>
/// The <see cref="RateLimitConstants" /> class.
/// </summary>
public static class RateLimitConstants
{
    /// <summary>
    /// The global policy name.
    /// </summary>
    public const string GlobalPolicyName = "global";

    /// <summary>
    /// Loads the rate limit options from the configuration, if the configuration does not exist
    /// </summary>
    public static IEnumerable<RateLimitOptions> LoadRateLimitOptions(IConfiguration configuration)
    {
        var options = new List<RateLimitOptions>();

        configuration.GetSection("RateLimitPolicies")
            .Bind(options);

        if (options.Count == 0)
        {
            Console.Error.WriteLine("No rate limit policies found in configuration, using default");
            options.Add(RateLimitOptions.Default);
        }

        return options;
    }
}