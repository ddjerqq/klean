using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filters;

/// <summary>
/// A filter that does not let the request pass, unless the model state is valid.
/// </summary>
public sealed class FluentValidationFilter : IActionFilter
{
    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var problem = new ValidationProblemDetails(context.ModelState);
            context.Result = new ObjectResult(problem);
        }
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}