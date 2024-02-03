using Application.Auth.Commands;

namespace Client.Services.Interfaces;

/// <summary>
/// Auth service for logging in, registering, logging out, etc.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Login with username and password
    /// </summary>
    public Task LoginAsync(UserLoginCommand command, CancellationToken ct = default);

    /// <summary>
    /// Register a new user
    /// </summary>
    public Task RegisterAsync(UserRegisterCommand command, CancellationToken ct = default);

    /// <summary>
    /// Logout
    /// </summary>
    public Task LogoutAsync(CancellationToken ct = default);
}