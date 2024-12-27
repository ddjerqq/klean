using Application.Services;
using Destructurama.Attributed;
using Domain.Aggregates;
using Domain.Entities;
using Domain.Events;
using EntityFrameworkCore.DataProtection.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LoginResult = (bool IsLockedOut, string? Token, Domain.Aggregates.User? User)?;

namespace Application.Cqrs.Users.Commands;

public sealed record LoginCommand : IRequest<LoginResult>
{
    [LogMasked]
    public string Email { get; set; } = null!;

    [LogMasked]
    public string Password { get; set; } = null!;

    public TimeZoneInfo TimeZoneInfo { get; set; } = null!;
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(12);
    }
}

internal sealed class LoginCommandHandler(
    IAppDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IJwtGenerator jwtGenerator)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users.WherePdEquals(nameof(User.Email), request.Email.ToLowerInvariant()).FirstOrDefaultAsync(ct);

        if (user is null)
            return null;

        if (user.LockoutEnd is not null && user.LockoutEnd >= DateTime.UtcNow)
            return (true, null, null);

        if (user.LockoutEnd is not null && user.LockoutEnd <= DateTime.UtcNow)
        {
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
        }

        if (user.AccessFailedCount >= User.MaxAccessFailedCount)
        {
            var lockoutEnd = user.LockoutEnd ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, user.TimeZone).Add(User.LockoutDuration);
            user.LockoutEnd = lockoutEnd;
            user.AddDomainEvent(new UserLockedOut(user.Id));
            await dbContext.SaveChangesAsync(ct);
            return (true, null, null);
        }

        if (!BC.EnhancedVerify(request.Password, user.PasswordHash))
        {
            user.AccessFailedCount++;
            await dbContext.SaveChangesAsync(ct);
            return null;
        }

        await UpdateLoginAsync(user, request.TimeZoneInfo, ct);

        var token = jwtGenerator.GenerateToken(user);
        return (false, token, user);
    }


    private async Task UpdateLoginAsync(User user, TimeZoneInfo timeZoneInfo, CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        var login = await dbContext.Set<UserLogin>()
            .Where(ul => ul.UserId == user.Id)
            .WherePdEquals(nameof(UserLogin.UserAgent), userAgent)
            .FirstOrDefaultAsync(ct);

        if (login is null)
        {
            login = new UserLogin(UserLoginId.New())
            {
                UserId = user.Id,
                UserAgent = userAgent,
                Location = "unknown",
                IpAddress = ipAddress ?? "unknown",
                LastActive = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo),
            };

            user.Logins.Add(login);
            user.AddDomainEvent(new UserLoggedInFromNewDevice(user.Id, login.Id));
        }
        else
        {
            login.LastActive = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
        }

        user.TimeZone = timeZoneInfo;

        await dbContext.SaveChangesAsync(ct);
    }
}