using Application.Common;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence.Interceptors;

public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
            return await base.SavingChangesAsync(eventData, result, ct);

        var outboxMessages = dbContext
            .ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
            .Where(entity => entity
                .GetType()
                .GetInterfaces()
                .Where(type => type.IsGenericType)
                .Any(type => type.GetGenericTypeDefinition() == typeof(IAggregateRoot<>)))
            .SelectMany(entity =>
            {
                if (((dynamic?)entity)?.DomainEvents is not IEnumerable<IDomainEvent> domainEvents)
                    return [];

                domainEvents = domainEvents.ToList();

                ((dynamic?)entity)?.ClearDomainEvents();

                return domainEvents;
            })
            .Select(OutboxMessage.FromDomainEvent)
            .ToList();

        await dbContext.Set<OutboxMessage>()
            .AddRangeAsync(outboxMessages, ct);

        return await base.SavingChangesAsync(eventData, result, ct);
    }
}