using System.Security.Claims;
using Domain.Aggregates;

namespace Application.Services;

/// <summary>
///     Represents a service that manages JWT tokens.
/// </summary>
public interface IJwtGenerator
{
    /// <summary>
    /// Generates a JWT token with the specified claims, expiration, and date time provider.
    /// </summary>
    public string GenerateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null);

    /// <summary>
    /// Generates a JWT token with the specified user
    /// </summary>
    public string GenerateToken(User user, TimeSpan? expiration = null);

    /// <summary>
    /// Tries to validate the specified token and returns the claims if the token is valid.
    /// </summary>
    public bool TryValidateToken(string token, out List<Claim> claims);
}