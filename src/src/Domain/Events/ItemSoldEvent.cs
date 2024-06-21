using Domain.Abstractions;
using Klean.Generated;

namespace Domain.Events;

public sealed record ItemSoldEvent(ItemId Id) : IDomainEvent;