using Domain.Aggregates;
using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record LoginEvent(UserId UserId, DateTime LoginTime)
    : IDomainEvent;