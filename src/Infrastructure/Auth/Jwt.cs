using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Domain.Common.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

/// <summary>
/// Jwt helper class, used to generate, validate and configure JWT tokens
/// </summary>
public static class Jwt
{
    internal static readonly JwtSecurityTokenHandler Handler = new();

    internal static readonly JwtBearerEvents Events = new()
    {
        OnMessageReceived = ctx =>
        {
            ctx.Request.Query.TryGetValue("authorization", out var query);
            ctx.Request.Headers.TryGetValue("authorization", out var header);
            ctx.Request.Cookies.TryGetValue("authorization", out var cookie);
            ctx.Token = (string?)query ?? (string?)header ?? cookie;
            return Task.CompletedTask;
        },
    };

    internal static readonly TimeSpan Expiration = TimeSpan.FromMinutes(int.Parse("JWT__EXPIRATION".FromEnv("60")));

    internal static readonly string ClaimsIssuer = "JWT__ISSUER".FromEnv("localhost");

    internal static readonly string ClaimsAudience = "JWT__AUDIENCE".FromEnv("localhost");

    internal static readonly string Key = "JWT__KEY".FromEnv() ?? throw new Exception("JWT__KEY is not set");

    internal static readonly SymmetricSecurityKey SecurityKey = new(Encoding.UTF8.GetBytes(Key));

    internal static readonly SigningCredentials SigningCredentials = new(SecurityKey, SecurityAlgorithms.HmacSha256);

    internal static readonly TokenValidationParameters TokenValidationParameters = new()
    {
        ValidIssuer = ClaimsIssuer,
        ValidAudience = ClaimsAudience,
        IssuerSigningKey = SecurityKey,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
    };

    /// <summary>
    /// Generates a JWT token from the claims
    /// </summary>
    /// <param name="claims">The claims to include in the JWT token</param>
    /// <param name="dateTimeProvider">The date time provider</param>
    /// <returns>The generated JWT token</returns>
    public static string GenerateToken(IEnumerable<Claim> claims, IDateTimeProvider dateTimeProvider)
    {
        var token = new JwtSecurityToken(
            ClaimsIssuer,
            ClaimsAudience,
            claims,
            expires: dateTimeProvider.UtcNow.Add(Expiration),
            signingCredentials: SigningCredentials);

        return Handler.WriteToken(token);
    }

    /// <summary>
    /// Verifies the validity of the token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool ValidateToken(string token)
    {
        try
        {
            Handler.ValidateToken(token, TokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}