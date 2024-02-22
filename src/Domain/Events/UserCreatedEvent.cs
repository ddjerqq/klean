using Domain.Aggregates;
using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record UserCreatedEvent(UserId UserId) : IDomainEvent;