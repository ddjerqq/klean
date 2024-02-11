using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Dtos;

[AutoMap(typeof(Item))]
public sealed record ItemDto
{
    public Guid Id { get; init; }

    public float Rarity { get; init; }

    public ItemType ItemType { get; init; } = default!;
}