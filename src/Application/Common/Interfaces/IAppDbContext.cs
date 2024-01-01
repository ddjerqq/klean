using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

/// <summary>
/// The <see cref="IAppDbContext" /> interface.
/// </summary>
public interface IAppDbContext : IDisposable
{
    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> of type <typeparamref name="TEntity"/> for this context.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, of which you want to get the DbSet of</typeparam>
    /// <returns>The DbSet of the type</returns>
    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Gets an <see cref="EntityEntry{TEntity}"/> for the given <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">The entity to get the EntityEntry for</param>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    /// <returns>The EntityEntry for the given entity</returns>
    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    public Task<int> SaveChangesAsync(CancellationToken ct = default);
}