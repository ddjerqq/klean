using Domain.Abstractions;
using Klean.Generated;

namespace Domain.Aggregates;

[StrongId(typeof(Ulid))]
public sealed class User(UserId id) : AggregateRoot<UserId>(id)
{
    public string Username { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string PasswordHash { get; init; } = default!;
}