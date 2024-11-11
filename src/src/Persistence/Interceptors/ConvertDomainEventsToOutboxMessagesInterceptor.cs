using Application.Common;
using Application.Services;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

        var dateTimeProvider = dbContext.GetService<IDateTimeProvider>();

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
            .Select(e => OutboxMessage.FromDomainEvent(e, dateTimeProvider))
            .ToList();

        await dbContext.Set<OutboxMessage>()
            .AddRangeAsync(outboxMessages, ct);

        return await base.SavingChangesAsync(eventData, result, ct);
    }
}