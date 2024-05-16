using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

public sealed class JwtAuthManager(HttpClient http) : AuthenticationStateProvider
{
    private static ClaimsPrincipal EmptyPrincipal => new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal = await GetUserClaimsAsync();
        return new AuthenticationState(claimsPrincipal);
    }

    private async Task<ClaimsPrincipal> GetUserClaimsAsync(CancellationToken ct = default)
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
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return EmptyPrincipal;
        }
    }
}