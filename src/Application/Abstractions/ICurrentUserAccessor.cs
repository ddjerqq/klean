using Domain.Aggregates;

namespace Application.Abstractions;

public interface ICurrentUserAccessor
{
    public Guid? CurrentUserId { get; }

    public Task<User?> GetCurrentUserAsync(CancellationToken ct = default);
}