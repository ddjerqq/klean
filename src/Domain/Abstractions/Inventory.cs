using Domain.Entities;

namespace Domain.Abstractions;

/// <summary>
/// Represents an inventory of items.
/// </summary>
public sealed class Inventory : Dictionary<Guid, Item>
{
    /// <summary>
    /// Gets the items in the inventory.
    /// </summary>
    public IEnumerable<Item> Items => this.Values;

    /// <summary>
    /// Checks if the inventory contains an item with the specified id.
    /// </summary>
    /// <param name="id">The id to check for</param>
    /// <returns>True if the inventory contains an item with the specified id, false otherwise</returns>
    public bool HasItemWithId(Guid id) => this.ContainsKey(id);

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    public void AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item.Owner);
        this.Add(item.Id, item);
    }

    /// <summary>
    /// Tries to remove an item from the inventory.
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    public bool TryRemoveItem(Item item)
    {
        item.Owner = null!;
        return this.Remove(item.Id);
    }

    /// <summary>
    /// Tries to transfer an item from this inventory to another.
    /// </summary>
    /// <note>This method does not update the owner of the transferred item</note>
    /// <param name="other">The inventory to transfer the item to</param>
    /// <param name="item">The item to transfer</param>
    /// <returns>True if the item was transferred, false otherwise</returns>
    public bool TryTransfer(Inventory other, Item item)
    {
        if (!HasItemWithId(item.Id))
            return false;

        TryRemoveItem(item);
        other.AddItem(item);

        return true;
    }
}