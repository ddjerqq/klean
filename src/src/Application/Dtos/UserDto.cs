using AutoMapper;
using Domain.Aggregates;

namespace Application.Dtos;

[AutoMap(typeof(User))]
public sealed class UserDto
{
    public UserId Id { get; init; }

    public string Username { get; init; } = default!;

    public decimal Balance { get; init; }

    public IEnumerable<ItemDto> Inventory { get; init; } = [];
}