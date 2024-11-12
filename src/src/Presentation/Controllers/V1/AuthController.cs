using Application.Common;
using Application.Cqrs.Users.Commands;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers.V1;

/// <summary>
/// Controller for authentication actions
/// </summary>
[Authorize]
public sealed class AuthController(ILogger<AuthController> logger, IAppDbContext dbContext, IMediator mediator) : ApiController(logger)
{
    /// <summary>
    /// Gets the user's claims
    /// </summary>
    [HttpGet("claims")]
    public ActionResult<Dictionary<string, string>> GetUserClaims() => Ok(User.Claims.ToDictionary(c => c.Type, c => c.Value));

    /// <summary>
    /// Gets the current user
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<User>> GetCurrentUser(CancellationToken ct)
    {
        var user = await dbContext.Users.FindAsync([User.GetId()], ct);
        return Ok(user);
    }

    /// <summary>
    /// Logs the user in
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(LoginCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);

        switch (response)
        {
            case LoginResponse.Success { Token: var token, User: var user }:
                Response.Cookies.Append("authorization", token);
                return Ok(user);
            // case LoginResponse.TwoFactorRequired:
            //     return Redirect()
            case LoginResponse.Failure:
                return BadRequest("bad credentials");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterCommand command, CancellationToken ct)
    {
        var (user, token) = await mediator.Send(command, ct);
        Response.Cookies.Append("authorization", token);
        return Ok(user);
    }

    /// <summary>
    /// Gets all users
    /// <note>This is only for Elon</note>
    /// </summary>
    [Authorize(Roles = RoleExt.Admin)]
    [HttpGet("all_users")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers(CancellationToken ct)
    {
        return Ok(await dbContext.Users.ToListAsync(ct));
    }
}