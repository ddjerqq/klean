using Domain.Abstractions;
using Domain.Entities;

namespace Domain.ValueObjects;

public sealed record ItemType(
    string Id,
    string Name,
    decimal Price,
    float MinRarity,
    float MaxRarity,
    string? DisplayUrl = null)
    : IValueObject
{
    public Item NewItem()
    {
        return new Item(ItemId.NewItemId())
        {
            Rarity = RandBetween(MinRarity, MaxRarity),
            ItemType = this,
        };
    }

    private static float RandBetween(float min, float max) =>
        (float)Random.Shared.NextDouble() * (max - min) + min;
}