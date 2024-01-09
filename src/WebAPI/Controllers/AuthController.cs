using Application.Auth.Commands;
using Application.Common.Interfaces;
using Domain.Aggregates;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using WebAPI.Common.Abstractions;

namespace WebAPI.Controllers;

internal sealed class AuthController : ApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, BindRequired] UserRegisterCommand command, CancellationToken ct)
    {
        var user = command.User;

        var success = await Mediator.Send(command, ct);

        if (!success)
            return BadRequest("username or email already exists");

        var token = user.GetToken(TimeSpan.FromHours(2), DateTimeProvider);
        Response.Cookies.Append("authorization", token, Cookie.Options);

        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody, BindRequired] UserLoginCommand command, CancellationToken ct)
    {
        var user = await Mediator.Send(command, ct);

        if (user is null)
            return BadRequest("bad credentials");

        var token = user.GetToken(
            command.RememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(2),
            DateTimeProvider);
        Response.Cookies.Append("authorization", token, Cookie.Options);

        return Ok(token);
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser(CancellationToken ct)
    {
        var currentUserId = GetService<ICurrentUserAccessor>()
            .CurrentUserId!
            .Value;

        var user = await DbContext.Set<User>()
            .AsNoTracking()
            .Include(x => x.Inventory)
            .ThenInclude(x => x.ItemType)
            .FirstOrDefaultAsync(x => x.Id == currentUserId, ct);

        user?.ChangePassword("not the password");

        return Ok(user);
    }

    [Authorize]
    [HttpGet("user_claims")]
    public IActionResult GetUserClaims()
    {
        return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    }
}