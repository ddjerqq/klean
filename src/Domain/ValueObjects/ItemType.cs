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
        return new Item(ItemId.NewItemId())
        {
            Rarity = RandBetween(MinRarity, MaxRarity),
            ItemType = this,
        };
    }

    private static float RandBetween(float min, float max) =>
        (float)Random.Shared.NextDouble() * (max - min) + min;

    public bool Equals(IValueObject? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;

        var otherItemType = (ItemType)other;
        return Id == otherItemType.Id
               && Name == otherItemType.Name
               && Price == otherItemType.Price
               && MinRarity.Equals(otherItemType.MinRarity)
               && MaxRarity.Equals(otherItemType.MaxRarity)
               && DisplayUrl == otherItemType.DisplayUrl;
    }
}