using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record ItemReceivedEvent(Guid ItemId) : IDomainEvent;