using System.Reflection;
using Application.Services;
using Domain.Abstractions;
using Domain.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;

namespace Persistence.Interceptors;

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

    // ReSharper disable once CognitiveComplexity
    private static void UpdateEntities(DbContext? context)
    {
        if (context is null)
            return;

        var userAccessor = context.GetService<ICurrentUserAccessor>();

        var currentUserId = userAccessor.Id?.ToString() ?? "system";
        var dateTime = DateTimeOffset.UtcNow;

        var trackedEntityEntries = context.ChangeTracker
            .Entries()
            .Where(x => x.Entity is ITrackedEntity);

        foreach (var entry in trackedEntityEntries)
        {
            var modifiedProperties = string.Join(", ", entry.Properties
                .Where(x => x.IsModified)
                .Select(x => x.Metadata.Name));

            if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                Log.Logger.Information("{UserId} {EntryState} entity {EntityId} {ModifiedProperties}", currentUserId, entry.State, ((dynamic)entry.Entity).Id, modifiedProperties);

            if (entry.State == EntityState.Added)
            {
                ((ITrackedEntity)entry.Entity).CreatedBy = currentUserId;
                ((ITrackedEntity)entry.Entity).Created = dateTime;
            }

            if (entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
            {
                ((ITrackedEntity)entry.Entity).LastModifiedBy = currentUserId;
                ((ITrackedEntity)entry.Entity).LastModified = dateTime;
            }

            if (entry.State == EntityState.Deleted)
            {
                // prevent deletion only if the entity has the SoftDeleteAttribute
                if (HasSoftDeleteAttribute(entry))
                    entry.State = EntityState.Modified;

                ((ITrackedEntity)entry.Entity).LastModifiedBy = currentUserId;
                ((ITrackedEntity)entry.Entity).LastModified = dateTime;

                ((ITrackedEntity)entry.Entity).DeletedBy = currentUserId;
                ((ITrackedEntity)entry.Entity).Deleted = dateTime;
            }
        }
    }

    private static bool HasSoftDeleteAttribute(EntityEntry entry)
    {
        return entry.Entity.GetType().GetCustomAttribute<SoftDeleteAttribute>() is not null;
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
    }
}