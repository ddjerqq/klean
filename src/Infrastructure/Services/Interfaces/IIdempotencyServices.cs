namespace Infrastructure.Services.Interfaces;

/// <summary>
/// cache of keys to check for idempotency
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Checks if the key exists in the cache
    /// </summary>
    public bool ContainsKey(Guid key);

    /// <summary>
    /// Adds the key to the cache
    /// </summary>
    public void AddKey(Guid key);
}