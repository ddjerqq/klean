using Application.Abstractions;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence.Interceptors;

public sealed class EntitySaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        UpdateEntities(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, ct);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context is null)
            return;

        var userAccessor = context.GetService<ICurrentUserAccessor>();
        var dateTimeProvider = context.GetService<IDateTimeProvider>();

        var currentUserId = userAccessor.CurrentUserId.ToString();
        var dateTime = dateTimeProvider.UtcNow;

        context.ChangeTracker
            .Entries<IEntity>()
            .ToList()
            .ForEach(entry =>
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.Created = dateTime;
                }

                if (entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
                {
                    entry.Entity.LastModifiedBy = currentUserId;
                    entry.Entity.LastModified = dateTime;
                }
            });
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}