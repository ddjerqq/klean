using Domain.Common.Abstractions;
using Domain.Entities;

namespace Domain.ValueObjects;

/// <summary>
/// Represents the type of an item.
/// </summary>
/// <param name="Id">Id of the item type</param>
/// <param name="Name">Name of item type</param>
/// <param name="Price">Price of item type</param>
/// <param name="MinRarity">Minimum rarity for this type of item</param>
/// <param name="MaxRarity">Maximum rarity for this type of item</param>
public sealed record ItemType(string Id, string Name, decimal Price, float MinRarity, float MaxRarity)
    : IValueObject
{
    /// <summary>
    /// Creates a new item of this type.
    /// </summary>
    public Item NewItem()
    {
        return new Item
        {
            Id = Guid.NewGuid(),
            Rarity = RandBetween(MinRarity, MaxRarity),
            ItemType = this,
        };
    }

    private static float RandBetween(float min, float max) =>
        ((float)Random.Shared.NextDouble() * (max - min)) + min;
}