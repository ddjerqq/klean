using System.Security.Claims;
using Application.Cqrs.Users;
using Application.Cqrs.Users.Commands;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers.V1;

/// <summary>
///     Controller for authentication actions
/// </summary>
[Authorize]
public sealed class AuthController(IAppDbContext dbContext, IOptions<IdentityOptions> identityOptions, IMediator mediator) : ApiController
{
    /// <summary>
    /// Gets the user's claims
    /// </summary>
    [HttpGet("claims")]
    public ActionResult<Dictionary<string, string>> GetUserClaims() => Ok(User.Claims.ToDictionary(c => c.Type, c => c.Value));

    /// <summary>
    ///     Gets the current user
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
    {
        var id = User.FindFirstValue(identityOptions.Value.ClaimsIdentity.UserIdClaimType)!;
        var userId = UserId.Parse(id);

        var user = await dbContext.Users.FindAsync([userId], ct);

        if (user is null)
            return NotFound();

        return Ok((UserDto)user);
    }

    /// <summary>
    ///     Logs the user in
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);

        switch (response)
        {
            case LoginResponse.Success { Token: var token, User: var user }:
                Response.Cookies.Append("authorization", token, Cookie.SecureOptions);
                return Ok((UserDto)user);
            case LoginResponse.Failure:
                return BadRequest("bad credentials");
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    ///     Register a new user
    /// </summary>
    /// <remarks>
    ///     Sample request:
    ///     POST /register
    ///     {
    ///     "email": "elon@gmail.com",
    ///     "fullname": "elonmusk",
    ///     "password": "supersecure
    ///     }
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterCommand command, CancellationToken ct)
    {
        var (user, token) = await mediator.Send(command, ct);
        Response.Cookies.Append("authorization", token);
        return Ok((UserDto)user);
    }

    [HttpPost("signout")]
    public Task<ActionResult> Signout()
    {
        Response.Cookies.Delete("authorization");
        return Task.FromResult<ActionResult>(Ok());
    }

    /// <summary>
    ///     Gets all users
    ///     <note>This is only for Elon</note>
    /// </summary>
    [Authorize(Roles = RoleExt.Admin)]
    [HttpGet("all_users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken ct)
    {
        var users = await dbContext.Users.ToListAsync(ct);
        return Ok(users.Select(user => (UserDto)user));
    }
}