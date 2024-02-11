using Domain.Entities;

namespace Domain.Abstractions;

public sealed class Inventory : List<Item>
{
    public bool HasItemWithId(Guid id) => this.Any(item => item.Id == id);

    public void AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item.Owner);
        this.Add(item);
    }

    public bool TryRemoveItem(Item item)
    {
        item.Owner = null!;
        return this.Remove(item);
    }

    public bool TryTransfer(Inventory other, Item item)
    {
        if (!HasItemWithId(item.Id))
            return false;

        TryRemoveItem(item);
        other.AddItem(item);

        return true;
    }
}