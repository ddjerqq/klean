using System.Security.Claims;

namespace Infrastructure.Common;

public static class ClaimsExt
{
    public static bool HasAllKeys(this IEnumerable<Claim> claims, params string[] keys) => keys.All(key => claims.Select(c => c.Type).Contains(key));

    public static string? FindFirstValue(this IEnumerable<Claim> claims, string type) => claims.FirstOrDefault(c => c.Type == type)?.Value;
}