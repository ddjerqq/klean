using System.Security.Claims;
using Domain.Abstractions;
using Domain.Aggregates;
using Generated;

namespace Domain.Entities;

[StrongId]
public sealed class UserClaim(UserClaimId id) : Entity<UserClaimId>(id)
{
    public User User { get; init; } = null!;
    public UserId UserId { get; init; }
    public required string ClaimType { get; init; }
    public required string ClaimValue { get; init; }

    public Claim Claim => new(ClaimType, ClaimValue);
}