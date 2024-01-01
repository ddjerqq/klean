using Domain.Entities;

namespace Domain.Aggregates;

/// <summary>
/// Represents an inventory of items.
/// </summary>
public sealed class Inventory
{
    private readonly User _owner;
    private readonly HashSet<Item> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="Inventory"/> class.
    /// </summary>
    /// <param name="owner">The owner of the inventory</param>
    /// <param name="items">The items in the inventory</param>
    public Inventory(User owner, IEnumerable<Item> items)
    {
        _owner = owner;
        _items = items.ToHashSet();
    }

    /// <summary>
    /// Gets the items in the inventory.
    /// </summary>
    public IEnumerable<Item> Items => _items.AsEnumerable();

    /// <summary>
    /// Checks if the inventory contains an item with the specified id.
    /// </summary>
    /// <param name="id">The id to check for</param>
    /// <returns>True if the inventory contains an item with the specified id, false otherwise</returns>
    public bool HasItemWithId(Guid id)
    {
        return _items.Any(x => x.Id == id);
    }

    /// <summary>
    /// Checks if the inventory contains an item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasItem(Item item)
    {
        return _items.Contains(item);
    }

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    public void AddItem(Item item)
    {
        item.Owner = _owner;
        _items.Add(item);
    }

    /// <summary>
    /// Tries to remove an item from the inventory.
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    public bool TryRemoveItem(Item item)
    {
        item.Owner = null!;
        return _items.Remove(item);
    }

    /// <summary>
    /// Tries to transfer an item from this inventory to another.
    /// </summary>
    /// <param name="other">The inventory to transfer the item to</param>
    /// <param name="item">The item to transfer</param>
    /// <returns>True if the item was transferred, false otherwise</returns>
    public bool TryTransfer(Inventory other, Item item)
    {
        if (!HasItem(item))
            return false;

        _items.Remove(item);
        other.AddItem(item);

        item.Owner = other._owner;

        return true;
    }
}