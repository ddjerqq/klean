using Application.Exceptions;
using Domain.Aggregates;

namespace Application.Services;

public interface ICurrentUserAccessor
{
    public UserId? Id { get; }
    public string? FullName { get; }
    public string? Email { get; }

    public Task<User?> TryGetCurrentUserAsync(CancellationToken ct = default);

    public async Task<User> GetCurrentUserAsync(CancellationToken ct = default)
    {
        return await TryGetCurrentUserAsync(ct) ?? throw new UnauthenticatedException("The user is not authenticated");
    }
}