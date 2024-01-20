using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebAPI.Services;

/// <inheritdoc />
public sealed class JwtAuthenticationStateProvider(HttpClient http)
    : AuthenticationStateProvider
{
    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal = await GetUserStateAsync();
        return new AuthenticationState(claimsPrincipal);
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