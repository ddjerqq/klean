using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Extensions;

internal static class DbSetExtensions
{
    internal static EntityEntry<TEntity>? TryUpdateIfNotNull<TEntity>(this DbSet<TEntity> dbSet, TEntity? entity)
        where TEntity : class
    {
        try
        {
            if (entity is not null)
            {
                return dbSet.Update(entity);
            }
        }
        catch (InvalidOperationException)
        {
            // the entity is already being tracked by the DbContext
            // common causes: the entity is the current user.
        }

        return null;
    }
}