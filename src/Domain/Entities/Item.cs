using Domain.Abstractions;
using Domain.Aggregates;
using Domain.ValueObjects;

namespace Domain.Entities;

public readonly record struct ItemId(Ulid Value)
{
    public static ItemId Empty => new(Ulid.Empty);
    public static ItemId NewItemId() => new(Ulid.NewUlid());
}

public sealed class Item(ItemId id) : Entity<ItemId>(id)
{
    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;

    public User Owner { get; set; } = default!;
}