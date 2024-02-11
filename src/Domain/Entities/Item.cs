using Domain.Aggregates;
using Domain.Common.Interfaces;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Item : EntityBase
{
    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;

    public User Owner { get; set; } = default!;
}