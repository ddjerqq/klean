using MediatR;

namespace Domain.Common.Abstractions;

/// <summary>
/// Represents the base DomainEvent.
/// use this to represent events, such as entity creation,
/// task completion and so on. Once raised the inbox pattern
/// handles acknowledging them.
/// </summary>
public interface IDomainEvent : INotification;