using Domain.Aggregates;

namespace Application.Services.Interfaces;

public interface ICurrentUserAccessor
{
    public UserId? CurrentUserId { get; }

    public Task<User?> GetCurrentUserAsync(CancellationToken ct = default);
}