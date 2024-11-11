using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Domain.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public static class JwtGenerator
{
    public static readonly string ClaimsIssuer = "JWT__ISSUER".FromEnvRequired();
    public static readonly string ClaimsAudience = "JWT__AUDIENCE".FromEnvRequired();
    private static readonly string Key = "JWT__KEY".FromEnvRequired();

    public static readonly JwtBearerEvents Events = new()
    {
        OnChallenge = ctx =>
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Found;
            ctx.Response.Redirect("/login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        },

        OnForbidden = ctx =>
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            ctx.Response.Redirect("/404");
            ctx.Fail("the requested resource could not be found");
            return Task.CompletedTask;
        },

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

    private static readonly JwtSecurityTokenHandler Handler = new();
    private static readonly SymmetricSecurityKey SecurityKey = new(Encoding.UTF8.GetBytes(Key));
    private static SigningCredentials SigningCredentials => new(SecurityKey, SecurityAlgorithms.HmacSha256);

    public static string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiration, IDateTimeProvider dateTimeProvider)
    {
        var token = new JwtSecurityToken(
            ClaimsIssuer,
            ClaimsAudience,
            claims,
            expires: dateTimeProvider.UtcNow.Add(expiration),
            signingCredentials: SigningCredentials);

        return Handler.WriteToken(token);
    }
}