using System.Net.Http.Json;
using System.Security.Claims;
using Application.Auth.Commands;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebUI.Services;

public sealed class JwtAuthenticationStateProvider(HttpClient http)
    : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal = await GetUserStateAsync();
        return new AuthenticationState(claimsPrincipal);
    }

    public async Task LoginAsync(UserLoginCommand command, CancellationToken ct = default)
    {
        var result = await http.PostAsJsonAsync("api/auth/login", command, ct);
        result.EnsureSuccessStatusCode();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        // TODO handle authentication here. set the user dto in localstorage
    }

    public async Task RegisterAsync(UserRegisterCommand command, CancellationToken ct = default)
    {
        var result = await http.PostAsJsonAsync("api/auth/register", command, ct);
        result.EnsureSuccessStatusCode();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal EmptyPrincipal => new(new ClaimsIdentity());

    private async Task<ClaimsPrincipal> GetUserStateAsync(CancellationToken ct = default)
    {
        var resp = await http.GetAsync("api/auth/user_claims", ct);
        if (!resp.IsSuccessStatusCode)
            return EmptyPrincipal;

        var body = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>(ct);

        if (body is not { Count: > 0 })
            return EmptyPrincipal;

        var claims = body
            .Select(kv => new Claim(kv.Key, kv.Value))
            .ToList();

        var claimsIdentity = new ClaimsIdentity(claims, "bearer");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return claimsPrincipal;
    }
}