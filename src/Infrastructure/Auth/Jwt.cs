using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Domain.Common.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

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

    internal static readonly string ClaimsIssuer = "JWT__ISSUER".FromEnv("klean");

    internal static readonly string ClaimsAudience = "JWT__AUDIENCE".FromEnv("klean");

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

    public static string GenerateToken(
        IEnumerable<Claim> claims,
        TimeSpan expiration,
        IDateTimeProvider dateTimeProvider)
    {
        var token = new JwtSecurityToken(
            ClaimsIssuer,
            ClaimsAudience,
            claims,
            expires: dateTimeProvider.UtcNow.Add(expiration),
            signingCredentials: SigningCredentials);

        return Handler.WriteToken(token);
    }

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