using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

internal static class JwtBearerOptions
{
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

    public static string ClaimsIssuer =>
        Environment.GetEnvironmentVariable("JWT__ISSUER")
        ?? throw new Exception("JWT__ISSUER is not present in the environment");

    public static string ClaimsAudience =>
        Environment.GetEnvironmentVariable("JWT__AUDIENCE")
        ?? throw new Exception("JWT__AUDIENCE is not present in the environment");

    public static string Key =>
        Environment.GetEnvironmentVariable("JWT__KEY")
        ?? throw new Exception("JWT__KEY is not present in the environment");

    public static SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes(Key));

    public static TokenValidationParameters TokenValidationParameters => new()
    {
        ValidateIssuer = Environment.GetEnvironmentVariable("JWT__ISSUER") is var issuer,
        ValidIssuer = issuer,
        ValidateAudience = Environment.GetEnvironmentVariable("JWT__AUDIENCE") is var audience,
        ValidAudience = audience,
        ValidAlgorithms = ["HS256"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SecurityKey,
        ValidateLifetime = true,
    };
}