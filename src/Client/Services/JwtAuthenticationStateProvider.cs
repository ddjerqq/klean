using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

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
        try
        {
            var body = await http.GetFromJsonAsync<Dictionary<string, string>>("api/auth/user_claims", ct);

            if (body is not { Count: > 0 })
                return EmptyPrincipal;

            var claims = body
                .Select(kv => new Claim(kv.Key, kv.Value))
                .ToList();

            var claimsIdentity = new ClaimsIdentity(claims, "bearer");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return claimsPrincipal;
        }
        catch (Exception)
        {
            return EmptyPrincipal;
        }
    }
}