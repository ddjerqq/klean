using System.Linq.Expressions;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Dto;

/// <inheritdoc cref="Domain.Entities.Item"/>
public sealed record ItemDto(
    Guid Id,
    float Rarity,
    ItemType ItemType)
{
    /// <summary>
    /// Implicit conversion from <see cref="Item"/> to <see cref="ItemDto"/>.
    /// </summary>
    public static implicit operator ItemDto(Item item) =>
        new(item.Id, item.Rarity, item.ItemType);

    /// <summary>
    /// Projection from <see cref="Item"/> to <see cref="ItemDto"/>.
    /// </summary>
    public static readonly Expression<Func<Item, ItemDto>> ProjectFrom = item =>
        new ItemDto(item.Id, item.Rarity, item.ItemType);
}