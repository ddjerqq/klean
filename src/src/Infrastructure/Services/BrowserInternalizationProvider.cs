using System.Globalization;
using Microsoft.JSInterop;

namespace Infrastructure.Services;

public sealed class BrowserInternalizationProvider(IJSRuntime js)
{
    /// <summary>
    /// Gets the user's browser's time zone
    /// </summary>
    /// <example>
    /// <code>
    /// var timeZone = await browserTimeZoneProvider.GetBrowserTimeZone();
    /// var date = TimeZoneInfo.ConvertTime(date, timeZone);
    /// </code>
    /// </example>
    public async Task<TimeZoneInfo> GetBrowserTimeZoneAsync(CancellationToken ct = default)
    {
        try
        {
            var value = await js.InvokeAsync<string>("window.getBrowserTimeZone", ct);
            return TimeZoneInfo.FindSystemTimeZoneById(value);
        }
        catch (InvalidOperationException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    /// <summary>
    /// Gets the user's browser's locale
    /// </summary>
    public async Task<CultureInfo> GetBrowserLocaleAsync(CancellationToken ct = default)
    {
        try
        {
            var value = await js.InvokeAsync<string>("window.getBrowserLocale", ct);
            return CultureInfo.GetCultureInfo(value);
        }
        catch (InvalidOperationException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}