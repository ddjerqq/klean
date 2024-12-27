using Domain.Abstractions;
using Domain.Aggregates;
using Domain.Entities;

namespace Domain.Events;

public sealed record UserLoggedInFromNewDevice(UserId UserId, UserLoginId UserLoginId) : IDomainEvent;