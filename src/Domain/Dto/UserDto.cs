using System.Linq.Expressions;
using Domain.Aggregates;

namespace Domain.Dto;

/// <inheritdoc cref="Domain.Aggregates.User"/>
public sealed record UserDto(
    Guid Id,
    string Username,
    string Email,
    string? ProfilePictureUrl,
    decimal Balance,
    IEnumerable<ItemDto> Inventory)
{
    /// <summary>
    /// Implicit conversion from <see cref="User"/> to <see cref="UserDto"/>.
    /// </summary>
    public static implicit operator UserDto(User user) =>
        new(user.Id,
            user.Username,
            user.Email,
            user.ProfilePictureUrl,
            user.Wallet.Balance,
            user.Inventory.Select(x => (ItemDto)x));

    /// <summary>
    /// Projection from <see cref="User"/> to <see cref="UserDto"/>.
    /// </summary>
    public static readonly Expression<Func<User, UserDto>> ProjectFrom = user =>
        new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.ProfilePictureUrl,
            user.Wallet.Balance,
            user.Inventory.Select(ItemDto.ProjectFrom.Compile()));
}