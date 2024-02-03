using Application.Abstractions;
using Application.Auth.Commands;
using Application.Dtos;
using AutoMapper.QueryableExtensions;
using Domain.Aggregates;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Presentation.Common.Abstractions;

namespace Presentation.Controllers;

/// <summary>
/// controller for authentication
/// </summary>
public sealed class AuthController : ApiController
{
    /// <summary>
    /// registers a new user
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, BindRequired] UserRegisterCommand command, CancellationToken ct)
    {
        var user = command.CreateUser();

        var success = await Mediator.Send(command, ct);

        if (!success)
            return BadRequest("username or email already exists");

        var token = user.GetToken(TimeSpan.FromHours(2), DateTimeProvider);
        Response.Cookies.Append("authorization", token, Cookie.Options);

        return Ok(token);
    }

    /// <summary>
    /// logs in a user
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody, BindRequired] UserLoginCommand command, CancellationToken ct)
    {
        var user = await Mediator.Send(command, ct);

        if (user is null)
            return BadRequest("bad credentials");

        var expiration = command.RememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(2);
        var token = user.GetToken(expiration, DateTimeProvider);
        Response.Cookies.Append("authorization", token, Cookie.Options);

        return Ok(token);
    }

    /// <summary>
    /// Gets the user's information
    /// </summary>
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
            .ProjectTo<UserDto>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == currentUserId, ct);

        return Ok(user);
    }

    /// <summary>
    /// Gets the user's claims
    /// </summary>
    [Authorize]
    [HttpGet("user_claims")]
    public IActionResult GetUserClaims()
    {
        return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    }
}