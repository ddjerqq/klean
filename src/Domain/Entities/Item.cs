using Domain.Aggregates;
using Domain.Common.Interfaces;
using Domain.ValueObjects;

namespace Domain.Entities;

public readonly record struct ItemId(Guid Value)
{
    public static ItemId Empty => new(Guid.Empty);
    public static ItemId NewItemId() => new(Guid.NewGuid());
}

public sealed class Item(ItemId id) : EntityBase<ItemId>(id)
{
    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;

    public User Owner { get; set; } = default!;
}