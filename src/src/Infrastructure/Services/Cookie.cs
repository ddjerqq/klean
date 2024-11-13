using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
///     cookie options
/// </summary>
public static class Cookie
{
    /// <summary>
    ///     options for secure cookies.
    /// </summary>
    public static readonly CookieOptions SecureOptions = new()
    {
        Domain = "WEB_APP__DOMAIN".FromEnvRequired(),
        MaxAge = TimeSpan.FromDays(1),
        Secure = true,
        HttpOnly = true,
        IsEssential = true,
        SameSite = SameSiteMode.None,
        Path = "/",
    };
}