using Domain.Abstractions;
using Domain.Aggregates;
using Domain.ValueObjects;
using Klean.Generated;

namespace Domain.Entities;

[StrongId(typeof(Ulid))]
public sealed class Item(ItemId id) : Entity<ItemId>(id)
{
    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;

    public User Owner { get; init; } = default!;
}