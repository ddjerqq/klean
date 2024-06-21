using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;
using Klean.Generated;

namespace Application.Dtos;

[AutoMap(typeof(Item))]
public sealed record ItemDto
{
    public ItemId Id { get; init; }

    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;
}