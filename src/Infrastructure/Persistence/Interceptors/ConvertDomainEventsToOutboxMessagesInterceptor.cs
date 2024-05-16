using Application.Common;
using Application.Services.Interfaces;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence.Interceptors;

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
        var aggregateRootType = typeof(IAggregateRoot<>);

        // this is a horrible hack... but it's a necessary evil,
        var outboxMessages = dbContext
            .ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
            .Where(entity => aggregateRootType.IsInstanceOfType(entity))
            .SelectMany(entity =>
            {
                if (((dynamic)entity)?.DomainEvents is not IEnumerable<IDomainEvent> domainEvents)
                    return Enumerable.Empty<IDomainEvent>();

                ((dynamic)entity)?.ClearDomainEvents();

                return domainEvents;
            })
            .Select(e => OutboxMessage.FromDomainEvent(e, dateTimeProvider))
            .ToList();

        await dbContext.Set<OutboxMessage>()
            .AddRangeAsync(outboxMessages, ct);

        return await base.SavingChangesAsync(eventData, result, ct);
    }
}