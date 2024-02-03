using AutoMapper;
using Domain.Aggregates;

namespace Application.Dtos;

/// <inheritdoc cref="Domain.Aggregates.User"/>
[AutoMap(typeof(User))]
public sealed class UserDto
{
    /// <inheritdoc cref="User.Id"/>
    public Guid Id { get; init; }

    /// <inheritdoc cref="User.Username"/>
    public string Username { get; init; } = default!;

    /// <inheritdoc cref="User.Email"/>
    public string Email { get; init; } = default!;

    /// <inheritdoc cref="User.ProfilePictureUrl"/>
    public string? ProfilePictureUrl { get; init; }

    /// <inheritdoc cref="User.Wallet"/>
    public decimal Balance { get; init; }

    /// <inheritdoc cref="User.Inventory"/>
    public IEnumerable<ItemDto> Inventory { get; init; } = [];
}