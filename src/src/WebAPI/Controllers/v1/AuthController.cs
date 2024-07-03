using Application.Dtos;
using Application.Services;
using Application.Users.Commands;
using Application.Users.Queries;
using AutoMapper;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1;

/// <summary>
/// controller for authentication
/// </summary>
[Authorize]
[Route("/api/v1/auth")]
public sealed class AuthController(IMediator mediator, IMapper mapper, IJwtGenerator jwtGenerator, ICurrentUserAccessor currentUserAccessor, IDateTimeProvider dateTimeProvider) : ApiController
{
    /// <summary>
    /// Get the current user
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var user = await currentUserAccessor.TryGetCurrentUserAsync();
        return Ok(mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Get the current user's claims
    /// </summary>
    [HttpGet("claims")]
    public ActionResult<IDictionary<string, string>> MyClaims()
    {
        var claims = HttpContext.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToDictionary();
        return Ok(claims);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] UserRegisterCommand command, CancellationToken ct)
    {
        var user = await mediator.Send(command, ct);
        var token = jwtGenerator.GenerateToken(JwtGenerator.GetUserClaims(user), TimeSpan.FromDays(1), dateTimeProvider);
        Response.Cookies.Append("authorization", token, Cookie.SecureOptions);
        return Ok(mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Log in
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginCommand command, CancellationToken ct)
    {
        var user = await mediator.Send(command, ct);

        if (user is null)
            return BadRequest("Invalid credentials");

        var token = jwtGenerator.GenerateToken(JwtGenerator.GetUserClaims(user), TimeSpan.FromDays(1), dateTimeProvider);
        Response.Cookies.Append("authorization", token, Cookie.SecureOptions);
        return Ok(mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Log out
    /// </summary>
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("authorization");
        return Ok();
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [AllowAnonymous]
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> Users([FromQuery] int page = 0, [FromQuery] int perPage = 25, CancellationToken ct = default)
    {
        var users = await mediator.Send(new GetAllUsersQuery(page, perPage), ct);
        return Ok(users.Select(mapper.Map<UserDto>));
    }
}