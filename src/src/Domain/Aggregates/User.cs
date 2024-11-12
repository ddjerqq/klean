using Destructurama.Attributed;
using Domain.Abstractions;
using Domain.ValueObjects;
using Generated;

namespace Domain.Aggregates;

[StrongId]
public sealed class User(UserId id) : AggregateRoot<UserId>(id)
{
    [LogMasked]
    public required string FullName { get; init; }

    [LogMasked]
    public required string Email { get; init; }

    [LogMasked]
    public required Role Role { get; init; }

    [LogMasked]
    public required string PasswordHash { get; init; }
}