using Domain.Abstractions;
using Domain.Entities;
using Domain.ValueObjects;
using Klean.Generated;

namespace Domain.Aggregates;

[StrongId(typeof(Ulid))]
public sealed class User(UserId id) : AggregateRoot<UserId>(id)
{
    public string Username { get; init; } = default!;

    public Wallet Wallet { get; init; } = default!;

    public ICollection<Item> Inventory { get; init; } = default!;
}