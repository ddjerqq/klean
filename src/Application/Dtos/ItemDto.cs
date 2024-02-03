using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Dtos;

/// <inheritdoc cref="Domain.Entities.Item"/>
[AutoMap(typeof(Item))]
public sealed record ItemDto
{
    /// <inheritdoc cref="Item.Id"/>
    public Guid Id { get; init; }

    /// <inheritdoc cref="Item.Rarity"/>
    public float Rarity { get; init; }

    /// <inheritdoc cref="Item.ItemType"/>
    public ItemType ItemType { get; init; } = default!;
}