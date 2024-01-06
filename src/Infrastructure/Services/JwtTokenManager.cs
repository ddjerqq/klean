using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

/// <summary>
/// The <see cref="JwtTokenManager" /> class, used to generate and verify JWT tokens.
/// </summary>
public static class JwtTokenManager
{
    private static readonly JwtSecurityTokenHandler Handler = new();

    private static SymmetricSecurityKey Key
    {
        get
        {
            string key = Environment.GetEnvironmentVariable("JWT__KEY")
                         ?? throw new ArgumentException("JWT__KEY is not present in the environment");

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }

    private static SigningCredentials Credentials => new(Key, SecurityAlgorithms.HmacSha256);

    private static TimeSpan Expiration
    {
        get
        {
            var expiration = Environment.GetEnvironmentVariable("JWT__EXPIRATION");

            return int.TryParse(expiration, out int minutes)
                ? TimeSpan.FromMinutes(minutes)
                : TimeSpan.FromDays(31);
        }
    }

    /// <summary>
    /// Generates a JWT token from the claims
    /// </summary>
    /// <param name="claims">The claims to include in the JWT token</param>
    /// <param name="dateTimeProvider">The date time provider</param>
    /// <returns>The generated JWT token</returns>
    public static string GenerateToken(IEnumerable<Claim> claims, IDateTimeProvider dateTimeProvider)
    {
        var token = new JwtSecurityToken(
            Environment.GetEnvironmentVariable("JWT__ISSUER"),
            Environment.GetEnvironmentVariable("JWT__AUDIENCE"),
            claims,
            expires: dateTimeProvider.UtcNow.Add(Expiration),
            signingCredentials: Credentials);

        return Handler.WriteToken(token);
    }

    /// <summary>
    /// Verifies the validity of the token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool VerifyToken(string token)
    {
        try
        {
            Handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT__ISSUER"),
                ValidAudience = Environment.GetEnvironmentVariable("JWT__AUDIENCE"),
                IssuerSigningKey = Key,
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}