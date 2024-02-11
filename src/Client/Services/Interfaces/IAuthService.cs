using Application.Auth.Commands;

namespace Client.Services.Interfaces;

public interface IAuthService
{
    public Task LoginAsync(UserLoginCommand command, CancellationToken ct = default);

    public Task RegisterAsync(UserRegisterCommand command, CancellationToken ct = default);

    public Task LogoutAsync(CancellationToken ct = default);
}