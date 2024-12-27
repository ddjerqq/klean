using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace Infrastructure.Services;

public sealed class CookieService(IJSRuntime js)
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

    public async Task<string> GetCookieAsync(string key, CancellationToken ct = default) =>
        await js.InvokeAsync<string>("window.getCookie", ct, key);

    public async Task SetCookieAsync(string key, string value, CancellationToken ct = default) =>
        await js.InvokeVoidAsync("window.setCookie", ct, key, value);

    public async Task DeleteCookieAsync(string key, CancellationToken ct = default) =>
        await js.InvokeVoidAsync("window.delCookie", ct, key);
}