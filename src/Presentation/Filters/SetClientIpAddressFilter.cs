using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

/// <summary>
/// A filter that sets the X-Client-IP to the request
/// </summary>
public sealed class SetClientIpAddressFilter : IActionFilter
{
    /// <inheritdoc />
    [SuppressMessage("Usage", "ASP0019", Justification = "This is a filter, not a controller action")]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var ipAddress =
            ExtractFromRealIpHeader(context.HttpContext)
            ?? ExtractFromForwardedForHeader(context.HttpContext)
            ?? context.HttpContext.Connection.RemoteIpAddress;

        if (ipAddress is not null)
            context.HttpContext.Request.Headers.Add("X-Client-IP", ipAddress.ToString());
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private static IPAddress? ExtractFromForwardedForHeader(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var xForwardedFor) || string.IsNullOrEmpty(xForwardedFor))
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