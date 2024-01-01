using Domain.Common.Abstractions;

namespace Domain.Events;

/// <summary>
/// Event for when an item is received.
/// </summary>
/// <param name="ItemId">The id of the item that was received.</param>
public sealed record ItemReceivedEvent(Guid ItemId) : IDomainEvent;