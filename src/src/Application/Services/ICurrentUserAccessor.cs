using Domain.Aggregates;
using Klean.Generated;

namespace Application.Services;

public interface ICurrentUserAccessor
{
    public UserId? CurrentUserId { get; }

    public Task<User?> GetCurrentUserAsync(CancellationToken ct = default);
}