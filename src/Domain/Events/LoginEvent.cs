using Domain.Common.Interfaces;

namespace Domain.Events;

/// <summary>
/// Event for when a new login is detected
/// </summary>
public sealed record LoginEvent(
    Guid UserId,
    DateTime LoginTime) : IDomainEvent;