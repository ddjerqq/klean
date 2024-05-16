using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public sealed class FluentValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var problem = new ValidationProblemDetails(context.ModelState);
            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Type = "https://httpstatuses.com/400",
                Status = StatusCodes.Status400BadRequest,
                Detail = problem.Detail,
                Extensions =
                {
                    ["traceId"] = context.HttpContext.TraceIdentifier,
                    ["errors"] = problem.Errors,
                    ["handler"] = "fluent_validation_filter",
                },
            };
            context.Result = new ObjectResult(problemDetails);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}