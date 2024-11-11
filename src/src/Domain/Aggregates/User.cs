using Destructurama.Attributed;
using Domain.Abstractions;
using Generated;

namespace Domain.Aggregates;

[StrongId]
public sealed class User(UserId id) : AggregateRoot<UserId>(id)
{
    public string Username { get; init; } = default!;

    public string Email { get; init; } = default!;

    [LogMasked]
    public string PasswordHash { get; init; } = default!;
}