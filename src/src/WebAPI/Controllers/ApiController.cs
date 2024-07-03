using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Controllers;

/// <summary>
/// Base class for all API controllers. has some boilerplate setup.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Gets the required service from the request services.
    /// </summary>
    protected T GetRequiredService<T>() where T : notnull => HttpContext.RequestServices.GetRequiredService<T>();
}