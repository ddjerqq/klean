using Domain.Common.Abstractions;
using Domain.Entities;

namespace Domain.Events;

/// <summary>
/// Event for when an item is sold.
/// </summary>
public sealed record ItemSoldEvent
    : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemSoldEvent"/> class.
    /// </summary>
    public ItemSoldEvent(Item item)
    {
        Item = new Item
        {
            Id = item.Id,
            Rarity = item.Rarity,

            // clone                 vvvvvvvv
            ItemType = item.ItemType with { },

            // do not include the owner, as it is not relevant to the event
            Owner = null!,
        };
    }

    /// <summary>
    /// Gets the time the item was sold.
    /// </summary>
    public DateTime SoldAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the item that was sold.
    /// </summary>
    public Item Item { get; init; }
}