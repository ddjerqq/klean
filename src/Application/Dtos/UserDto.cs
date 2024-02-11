using AutoMapper;
using Domain.Aggregates;

namespace Application.Dtos;

[AutoMap(typeof(User))]
public sealed class UserDto
{
    public Guid Id { get; init; }

    public string Username { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string? ProfilePictureUrl { get; init; }

    public decimal Balance { get; init; }

    public IEnumerable<ItemDto> Inventory { get; init; } = [];
}