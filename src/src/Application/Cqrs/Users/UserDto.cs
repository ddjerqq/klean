using Domain.Aggregates;
using Domain.ValueObjects;

namespace Application.Cqrs.Users;

public sealed record UserDto(UserId Id, string FullName, string Email, string? AvatarUrl, Role Role, string SecurityStamp)
{
    public static implicit operator UserDto(User user) => new(user.Id, user.FullName, user.Email, user.AvatarUrl, user.Role, user.SecurityStamp);
}