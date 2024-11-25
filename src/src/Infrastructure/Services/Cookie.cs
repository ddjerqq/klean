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
        MaxAge = TimeSpan.FromDays(1),
        HttpOnly = true,
        IsEssential = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
    };
}