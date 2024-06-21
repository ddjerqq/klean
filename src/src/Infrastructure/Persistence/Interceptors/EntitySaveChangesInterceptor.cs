using Application.Services;
using Domain.Abstractions;
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

        var currentUserId = userAccessor.CurrentUserId?.ToString();
        var dateTime = dateTimeProvider.UtcNow;

        var entityType = typeof(IEntity<>);

        // this is a horrible hack... but it's a necessary evil,
        context.ChangeTracker
            .Entries()
            .Select(entry => (Entry: entry, entry.Entity))
            .Where(x => entityType.IsInstanceOfType(x.Entity))
            .ToList()
            .ForEach(x =>
            {
                if (x.Entry.State == EntityState.Added)
                {
                    ((dynamic?)x.Entry.Entity)!.CreatedBy = currentUserId!;
                    ((dynamic?)x.Entry.Entity)!.Created = dateTime;
                }

                if (x.Entry.State == EntityState.Modified || HasChangedOwnedEntities(x.Entry))
                {
                    ((dynamic?)x.Entry.Entity)!.LastModifiedBy = currentUserId!;
                    ((dynamic?)x.Entry.Entity)!.LastModified = dateTime;
                }
            });
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}