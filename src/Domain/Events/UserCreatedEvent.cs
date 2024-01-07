using Domain.Common.Interfaces;

namespace Domain.Events;

/// <summary>
/// Event for when a user is created.
/// </summary>
public sealed record UserCreatedEvent(Guid UserId) : IDomainEvent;