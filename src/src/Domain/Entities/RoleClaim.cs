using System.Security.Claims;
using Domain.Abstractions;
using Generated;

namespace Domain.Entities;

[StrongId]
public sealed class RoleClaim(RoleClaimId id) : Entity<RoleClaimId>(id)
{
    public Role Role { get; set; } = null!;
    public RoleId RoleId { get; init; }
    public required string ClaimType { get; init; }
    public required string ClaimValue { get; init; }

    public Claim Claim => new(ClaimType, ClaimValue);
}