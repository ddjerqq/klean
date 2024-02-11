using System.Net.Http.Json;
using System.Security.Claims;
using Application.Auth.Commands;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

public sealed class JwtAuthManager(HttpClient http) : AuthenticationStateProvider, IAuthService
{
    private static ClaimsPrincipal EmptyPrincipal => new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal = await GetUserClaimsAsync();
        return new AuthenticationState(claimsPrincipal);
    }

    public async Task LoginAsync(UserLoginCommand command, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync("api/auth/login", command, ct);
        resp.EnsureSuccessStatusCode();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task RegisterAsync(UserRegisterCommand command, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync("api/auth/register", command, ct);
        resp.EnsureSuccessStatusCode();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var resp = await http.PostAsync("api/auth/logout", null, ct);
        resp.EnsureSuccessStatusCode();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
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