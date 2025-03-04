using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;
using Application.Services;
using Domain.Aggregates;
using Domain.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Infrastructure.Services;

public sealed class JwtGenerator : IJwtGenerator
{
    public const string CookieName = "authorization";

    public static readonly string ClaimsIssuer = "JWT__ISSUER".FromEnvRequired();
    public static readonly string ClaimsAudience = "JWT__AUDIENCE".FromEnvRequired();
    private static readonly string Key = "JWT__KEY".FromEnvRequired();

    public static readonly JwtBearerEvents Events = new()
    {
        OnMessageReceived = ctx =>
        {
            ctx.Request.Query.TryGetValue(CookieName, out var query);
            ctx.Request.Headers.TryGetValue(CookieName, out var header);
            ctx.Request.Cookies.TryGetValue(CookieName, out var cookie);
            ctx.Token = (string?)query ?? (string?)header ?? cookie;
            return Task.CompletedTask;
        },
        OnForbidden = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = 403;
                return Task.CompletedTask;
            }

            ctx.Response.Redirect("auth/denied");
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            ctx.Response.Redirect("auth/login");
            ctx.HandleResponse();
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

    public string GenerateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
    {
        expiration ??= TimeSpan.FromSeconds(int.Parse("JWT__EXPIRATION".FromEnvRequired()));

        var token = new JwtSecurityToken(
            ClaimsIssuer,
            ClaimsAudience,
            claims,
            expires: DateTime.UtcNow.Add(expiration.Value),
            signingCredentials: SigningCredentials);

        return _handler.WriteToken(token);
    }

    public string GenerateToken(User user, TimeSpan? expiration = null)
    {
        var claims = user.GetAllClaims();
        return GenerateToken(claims, expiration);
    }

    public bool TryValidateToken(string token, out List<Claim> claims)
    {
        try
        {
            var principal = _handler.ValidateToken(token, TokenValidationParameters, out _);
            claims = principal.Claims.ToList();
            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Debug(ex, "Failed to validate token");

            claims = [];
            return false;
        }
    }
}