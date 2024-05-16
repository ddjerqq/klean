using Domain.Abstractions;
using Domain.Entities;

namespace Domain.Events;

public sealed record ItemSoldEvent(ItemId Id) : IDomainEvent;