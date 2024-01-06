using Application.Common.Interfaces;

namespace Infrastructure.RateLimiting;

internal sealed record RateLimitCacheEntry(int Remaining, TimeSpan Per)
{
    public int Remaining { get; private set; } = Remaining;

    public bool TryAcquire(IDateTimeProvider dateTimeProvider, out DateTime retryAfter)
    {
        retryAfter = default;

        if (Remaining > 0)
        {
            Remaining--;
            return true;
        }

        retryAfter = dateTimeProvider.UtcNow + Per;
        return false;
    }
}