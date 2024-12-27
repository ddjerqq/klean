using Domain.Abstractions;
using Domain.Aggregates;

namespace Domain.Events;

public sealed record UserLockedOut(UserId UserId) : IDomainEvent;