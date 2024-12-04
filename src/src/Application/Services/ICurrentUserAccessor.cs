using Application.Exceptions;
using Domain.Aggregates;
using Domain.ValueObjects;

namespace Application.Services;

public interface ICurrentUserAccessor
{
    public UserId? Id { get; }
    public string? Username { get; }
    public string? Email { get; }
    public string? Avatar { get; }
    public Role? Role { get; }
    public string SecurityStamp { get; }

    public Task<User?> TryGetCurrentUserAsync(CancellationToken ct = default);

    public async Task<User> GetCurrentUserAsync(CancellationToken ct = default) =>
        await TryGetCurrentUserAsync(ct) ?? throw new UnauthenticatedException("The user is not authenticated");
}