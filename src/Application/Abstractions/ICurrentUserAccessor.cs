using Domain.Aggregates;

namespace Application.Abstractions;

/// <summary>
/// The <see cref="ICurrentUserAccessor" /> interface.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets the current user id, if an authenticated user exists,
    /// this property will return their id
    /// </summary>
    public Guid? CurrentUserId { get; }

    /// <summary>
    /// Gets the current user, if any authenticated user exists.
    /// </summary>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The current user, if any</returns>
    public Task<User?> GetCurrentUserAsync(CancellationToken ct = default);
}