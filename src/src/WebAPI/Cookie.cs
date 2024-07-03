using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace WebAPI;

/// <summary>
/// cookie options
/// </summary>
public static class Cookie
{
    /// <summary>
    /// options for secure cookies.
    /// </summary>
    public static readonly CookieOptions SecureOptions = new()
    {
        Domain = "WEB_APP__DOMAIN".FromEnv(),
        MaxAge = TimeSpan.FromDays(1),
        Secure = true,
        HttpOnly = true,
        IsEssential = true,
        SameSite = SameSiteMode.None,
        Path = "/",
    };
}