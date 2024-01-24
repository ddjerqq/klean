using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Abstractions;

namespace Presentation.Controllers;

/// <summary>
/// controller for authentication
/// </summary>
public sealed class HomeController : ApiController
{
    /// <summary>
    /// Test endpoint
    /// </summary>
    [HttpGet("/")]
    public IActionResult Index()
    {
        return Ok("hello world");
    }
}