using System.ComponentModel;
using AutoMapper;
using Domain.Aggregates;

namespace Domain.Dto;

/// <inheritdoc cref="Domain.Aggregates.User"/>
public sealed record UserDto(
    Guid Id,
    string Username,
    string Email,
    string? ProfilePictureUrl,
    decimal Balance,
    IEnumerable<ItemDto> Inventory);

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class UserDtoMappingProfile : Profile
{
    public UserDtoMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}