using System.Net.Http.Json;
using System.Text.Json;
using Application.Cqrs.Users;
using Application.Cqrs.Users.Commands;

namespace Client.Services;

public sealed class ApiService(HttpClient http, JsonSerializerOptions jsonOptions)
{
    public async Task<Dictionary<string, string>?> GetClaims(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<Dictionary<string, string>>("/api/v1/auth/claims", jsonOptions, ct);

    public async Task<UserDto?> GetMe(CancellationToken ct = default) => await http.GetFromJsonAsync<UserDto>("/api/v1/auth/me", jsonOptions, ct);

    public async Task<UserDto?> PostLogin(LoginCommand command, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync("/api/v1/auth/login", command, jsonOptions, ct);
        return await resp.Content.ReadFromJsonAsync<UserDto>(jsonOptions, ct);
    }

    public async Task<UserDto?> PostRegister(RegisterCommand command, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync("/api/v1/auth/register", command, jsonOptions, ct);
        return await resp.Content.ReadFromJsonAsync<UserDto>(jsonOptions, ct);
    }

    public async Task<IEnumerator<UserDto>?> GetAllUsers(CancellationToken ct = default) => await http.GetFromJsonAsync<IEnumerator<UserDto>>("/api/v1/users", jsonOptions, ct);
}