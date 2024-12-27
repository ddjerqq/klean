using Domain.Aggregates;

namespace Domain.Entities;

public sealed class UserRole
{
    public User User { get; set; } = null!;
    public UserId UserId { get; set; }

    public Role Role { get; set; } = null!;
    public RoleId RoleId { get; set; }
}