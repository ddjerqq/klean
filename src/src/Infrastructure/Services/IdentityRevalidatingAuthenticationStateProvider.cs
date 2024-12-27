using System.Security.Claims;
using Application.Common;
using Application.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public sealed class IdentityRevalidatingAuthenticationStateProvider(
    ILoggerFactory loggerFactory,
    IServiceScopeFactory scopeFactory)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(5);

    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUserAccessor>();

        var user = await currentUser.TryGetCurrentUserAsync(ct);
        if (user is null)
            return false;

        return authenticationState.User.GetSecurityStamp() == user.SecurityStamp;
    }

    public void ForceSignOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var anonymousState = new AuthenticationState(anonymousUser);
        SetAuthenticationState(Task.FromResult(anonymousState));
    }
}