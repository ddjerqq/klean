using Domain.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Auth;

public static class Cookie
{
    public static readonly CookieOptions Options = new()
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