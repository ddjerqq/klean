using Domain.Common.Interfaces;
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
        return new Item(Guid.NewGuid())
        {
            Rarity = RandBetween(MinRarity, MaxRarity),
            ItemType = this,
        };
    }

    private static float RandBetween(float min, float max) =>
        (float)Random.Shared.NextDouble() * (max - min) + min;
}