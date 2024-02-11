using Domain.Common.Interfaces;

namespace Domain.Events;

public sealed record LoginEvent(Guid UserId, DateTime LoginTime)
    : IDomainEvent;