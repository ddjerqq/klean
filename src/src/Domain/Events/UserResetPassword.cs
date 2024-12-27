using Domain.Abstractions;
using Domain.Aggregates;

namespace Domain.Events;

public sealed record UserResetPassword(UserId UserId) : IDomainEvent;