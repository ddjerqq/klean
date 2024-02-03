using Domain.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Auth;

/// <summary>
/// Cookie configuration
/// </summary>
public static class Cookie
{
    /// <summary>
    /// The default cookie options
    /// </summary>
    public static readonly CookieOptions Options = new()
    {
        Domain = "WEB_APP__DOMAIN".FromEnv(),
        MaxAge = TimeSpan.FromDays(1),
        Secure = true,
        HttpOnly = true,
        IsEssential = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
    };
}