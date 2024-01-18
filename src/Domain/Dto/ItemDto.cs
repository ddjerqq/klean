using System.ComponentModel;
using AutoMapper;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Dto;

/// <inheritdoc cref="Domain.Entities.Item"/>
public sealed record ItemDto(
    Guid Id,
    float Rarity,
    ItemType ItemType);

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class ItemDtoMappingProfile : Profile
{
    public ItemDtoMappingProfile()
    {
        CreateMap<Item, ItemDto>();
    }
}