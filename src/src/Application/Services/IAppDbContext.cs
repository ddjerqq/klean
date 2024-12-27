using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Services;

public interface IAppDbContext : IDisposable
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TEntity> Set<TEntity>() where TEntity : class;
    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    public Task<int> SaveChangesAsync(CancellationToken ct = default);
}