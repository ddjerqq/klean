using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

/// <inheritdoc />
public sealed class SetClientIpAddressFilter : IActionFilter
{
    private const string ClientIpItemName = "client_ip_address";

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    [SuppressMessage("Usage", "ASP0019", Justification = "This is a filter, not a controller action")]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var ipAddress =
            ExtractFromRealIpHeader(context.HttpContext)
            ?? ExtractFromForwardedForHeader(context.HttpContext)
            ?? context.HttpContext.Connection.RemoteIpAddress;

        if (ipAddress is null) return;

        context.HttpContext.Request.Headers.TryAdd("X-Real-IP", ipAddress.ToString());
        context.HttpContext.Items[ClientIpItemName] = ipAddress.ToString();
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private static IPAddress? ExtractFromForwardedForHeader(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var xForwardedFor) || string.IsNullOrWhiteSpace(xForwardedFor))
            return null;

        return xForwardedFor
            .FirstOrDefault()!
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Select(ip => new { Valid = IPAddress.TryParse(ip, out var addr), Addr = addr })
            .Where(x => x.Valid)
            .Select(x => x.Addr)
            .FirstOrDefault(address => address!.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6);
    }

    private static IPAddress? ExtractFromRealIpHeader(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue("X-Real-IP", out var xRealIp);

        if (IPAddress.TryParse(xRealIp, out var ipAddress)
            && ipAddress.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6)
            return ipAddress;

        return null;
    }
}