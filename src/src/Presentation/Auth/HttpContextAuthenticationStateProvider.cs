using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Presentation.Auth;

/// <inheritdoc />
public sealed class HttpContextAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    private static AuthenticationState Empty => new(new ClaimsPrincipal());

    /// <inheritdoc />
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (httpContextAccessor.HttpContext?.User is { } principal)
            return Task.FromResult(new AuthenticationState(principal));

        return Task.FromResult(Empty);
    }
}