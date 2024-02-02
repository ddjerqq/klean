using System.Security.Claims;
using Application.Abstractions;
using Domain.Aggregates;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Auth;

/// <summary>
/// The user extensions for things like Claims property.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Get the claims for this user.
    /// </summary>
    public static IEnumerable<Claim> GetClaims(this User user) =>
    [
        new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, user.Username),
    ];

    /// <summary>
    /// Get the token for this user.
    /// </summary>
    public static string GetToken(this User user, TimeSpan expiration, IDateTimeProvider dateTimeProvider) =>
        Jwt.GenerateToken(user.GetClaims(), expiration, dateTimeProvider);
}