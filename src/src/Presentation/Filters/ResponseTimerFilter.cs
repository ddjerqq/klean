using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Presentation.Filters;

public sealed class ResponseTimeFilter : IActionFilter
{
    private const string ResponseTimeKey = "response_time";
    private const string ResponseTimeHeader = "X-Response-Time";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items[ResponseTimeKey] = Stopwatch.StartNew();
    }

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