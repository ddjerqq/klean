using Domain.Abstractions;
using Domain.Aggregates;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public readonly record struct ItemId(Ulid Value)
{
    public static ItemId Empty => new(Ulid.Empty);
    public static ItemId NewItemId() => new(Ulid.NewUlid());

    public override string ToString() => StrongIdHelper<ItemId, Ulid>.Serialize(Value);

    public static ItemId? TryParse(string? value) => StrongIdHelper<ItemId, Ulid>.Deserialize(value, null);

    public static ItemId Parse(string value) =>
        StrongIdHelper<ItemId, Ulid>.Deserialize(value, null)
        ?? throw new FormatException("Input string was not in the correct format");
}

public sealed class Item(ItemId id) : Entity<ItemId>(id)
{
    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;

    public User Owner { get; set; } = default!;
}