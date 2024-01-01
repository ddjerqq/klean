using Domain.Aggregates;
using Domain.Common.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Represents an item in the game.
/// </summary>
public sealed class Item : EntityBase
{
    /// <summary>
    /// Gets or sets the rarity of the item.
    /// this value is between 0 and 1.
    /// with lower values being more rare.
    /// </summary>
    public float Rarity { get; set; }

    /// <summary>
    /// Gets the item's type.
    /// </summary>
    public ItemType ItemType { get; init; } = default!;

    /// <summary>
    /// Gets or sets the item's owner
    /// </summary>
    public User Owner { get; set; } = default!;
}