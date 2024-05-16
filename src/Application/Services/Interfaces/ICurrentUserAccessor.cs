using Domain.Aggregates;

namespace Application.Services.Interfaces;

public interface ICurrentUserAccessor
{
    public Guid? CurrentUserId { get; }

    public Task<User?> GetCurrentUserAsync(CancellationToken ct = default);
}