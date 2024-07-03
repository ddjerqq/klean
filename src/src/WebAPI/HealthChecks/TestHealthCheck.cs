using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebAPI.HealthChecks;

/// <summary>
/// A test healthcheck for you to implement
/// </summary>
public sealed class TestHealthCheck : IHealthCheck
{
    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("the app is healthy"));
    }
}