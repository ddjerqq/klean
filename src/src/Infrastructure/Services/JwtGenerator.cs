using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Domain.Aggregates;
using Domain.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

/// <inheritdoc />
public sealed class JwtGenerator : IJwtGenerator
{
    public static readonly string ClaimsIssuer = "JWT__ISSUER".FromEnv("ruby");
    public static readonly string ClaimsAudience = "JWT__AUDIENCE".FromEnv("ruby");
    private static readonly string Key = "JWT__KEY".FromEnvRequired();

    public static readonly JwtBearerEvents Events = new()
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

    public static readonly TokenValidationParameters TokenValidationParameters = new()
    {
        ValidIssuer = ClaimsIssuer,
        ValidAudience = ClaimsAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
    };

    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly SymmetricSecurityKey _securityKey = new(Encoding.UTF8.GetBytes(Key));
    private SigningCredentials SigningCredentials => new(_securityKey, SecurityAlgorithms.HmacSha256);

    public string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiration, IDateTimeProvider dateTimeProvider)
    {
        var token = new JwtSecurityToken(
            ClaimsIssuer,
            ClaimsAudience,
            claims,
            expires: dateTimeProvider.UtcNow.Add(expiration),
            signingCredentials: SigningCredentials);

        return _handler.WriteToken(token);
    }

    public static IEnumerable<Claim> GetUserClaims(User user)
    {
        return new[]
        {
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            // new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // new Claim("role", user.Level.DisplayName),
        };
    }
}