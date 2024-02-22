using Domain.Common.Interfaces;
using Domain.Entities;

namespace Domain.Events;

public sealed record ItemSoldEvent
    : IDomainEvent
{
    public ItemSoldEvent(Item item)
    {
        Item = new Item(item.Id)
        {
            Rarity = item.Rarity,

            // clone                 vvvvvvvv
            ItemType = item.ItemType with { },

            // do not include the owner, as it is not relevant to the event
            Owner = null!,
        };
    }

    public DateTime SoldAt { get; set; }

    public Item Item { get; init; }
}