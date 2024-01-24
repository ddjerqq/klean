using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Common.Abstractions;

/// <summary>
/// The base controller for API controllers
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("/api/[controller]")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Get a service of type
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    protected T GetService<T>() where T : notnull => HttpContext.RequestServices.GetRequiredService<T>();

    /// <summary>
    /// Gets the DbContext
    /// </summary>
    protected IAppDbContext DbContext => GetService<IAppDbContext>();

    /// <summary>
    /// Gets the Mediator
    /// </summary>
    protected IMediator Mediator => GetService<IMediator>();

    /// <summary>
    /// Gets the DateTimeProvider
    /// </summary>
    protected IDateTimeProvider DateTimeProvider => GetService<IDateTimeProvider>();
}