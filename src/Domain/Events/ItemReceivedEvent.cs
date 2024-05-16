using Domain.Abstractions;
using Domain.Entities;

namespace Domain.Events;

public sealed record ItemReceivedEvent(ItemId ItemId) : IDomainEvent;