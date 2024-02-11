using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record UserCreatedEvent(Guid UserId) : IDomainEvent;