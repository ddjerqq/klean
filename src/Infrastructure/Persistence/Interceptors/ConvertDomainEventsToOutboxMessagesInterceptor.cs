using Application.Common;
using Application.Common.Interfaces;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence.Interceptors;

/// <inheritdoc />
public class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        DbContext? dbContext = eventData.Context;

        if (dbContext is null)
            return await base.SavingChangesAsync(eventData, result, ct);

        var dateTimeProvider = dbContext.GetService<IDateTimeProvider>();

        var outboxMessages = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(e => OutboxMessage.FromDomainEvent(e, dateTimeProvider))
            .ToList();

        await dbContext.Set<OutboxMessage>()
            .AddRangeAsync(outboxMessages, ct);

        return await base.SavingChangesAsync(eventData, result, ct);
    }
}