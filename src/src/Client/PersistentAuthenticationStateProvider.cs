using System.Security.Claims;
using Application.Common;
using Application.Cqrs.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client;

internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> DefaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> _authenticationStateTask = DefaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserDto>(nameof(UserDto), out var userDto) || userDto is null)
            return;

        _authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(userDto.GetClaims(),
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}