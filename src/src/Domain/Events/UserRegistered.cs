using Domain.Abstractions;
using Domain.Aggregates;

namespace Domain.Events;

public sealed record UserRegistered(UserId UserId) : IDomainEvent;
