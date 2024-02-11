using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Abstractions;

public interface IAppDbContext : IDisposable
{
    public DbSet<TEntity> Set<TEntity>() where TEntity : class;

    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    public Task<int> SaveChangesAsync(CancellationToken ct = default);
}