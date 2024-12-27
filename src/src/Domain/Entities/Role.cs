using Domain.Abstractions;
using Generated;

namespace Domain.Entities;

[StrongId]
public sealed class Role(RoleId id) : Entity<RoleId>(id)
{
    public required string Name { get; init; }
    public required string ConcurrencyStamp { get; init; }

    public ICollection<RoleClaim> Claims { get; init; } = [];
}