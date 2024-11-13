using System.Security.Claims;

namespace Application.Services;

/// <summary>
///     Represents a service that manages JWT tokens.
/// </summary>
public interface IJwtGenerator
{
    /// <summary>
    ///     Generates a JWT token with the specified claims, expiration, and date time provider.
    /// </summary>
    public string GenerateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null);
}