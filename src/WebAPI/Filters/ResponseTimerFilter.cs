using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace WebAPI.Filters;

/// <summary>
/// A filter that measures how much time a request takes,
/// and adds the X-Response-Time header to the response
/// HH.MM.SS.fffffff
/// </summary>
public sealed class ResponseTimeFilter : IActionFilter
{
    private const string ResponseTimeKey = "response_time";
    private const string ResponseTimeHeader = "X-Response-Time";

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items[ResponseTimeKey] = Stopwatch.StartNew();
    }

    /// <inheritdoc />
    [SuppressMessage("Usage", "ASP0019", Justification = "This is a filter, not a controller action")]
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Items[ResponseTimeKey] is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var elapsed = $"{stopwatch.Elapsed:c}";
            context.HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>(ResponseTimeHeader, elapsed));
        }
    }
}