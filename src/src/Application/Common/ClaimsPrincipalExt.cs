using System.Security.Claims;
using Domain.Aggregates;
using Domain.ValueObjects;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Application.Common;

public static class ClaimsPrincipalExt
{
    private static string? GetClaimValue(this ClaimsPrincipal principal, string claimType) => principal
        .Claims
        .FirstOrDefault(x => x.Type == claimType)?
        .Value;

    public static UserId? GetId(this ClaimsPrincipal principal) =>
        UserId.TryParse(principal.GetClaimValue(JwtRegisteredClaimNames.Sid), null, out var id) ? id : null;

    public static string? GetFullName(this ClaimsPrincipal principal) =>
        principal.GetClaimValue(JwtRegisteredClaimNames.Name);

    public static string? GetEmail(this ClaimsPrincipal principal) =>
        principal.GetClaimValue(JwtRegisteredClaimNames.Email);

    public static Role? GetRole(this ClaimsPrincipal principal) =>
        int.TryParse(principal.GetClaimValue(nameof(User.Role)), out var value) ? (Role)value : null;

    public static bool HasAnyRole(this ClaimsPrincipal principal, params Role[] roles) =>
        principal.GetRole() is { } role &&
        roles.ToArray().Contains(role);

    public static IEnumerable<Claim> GetClaims(this User user)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(nameof(User.Role), ((int)user.Role).ToString()),
            // new Claim(nameof(User.Programme), ((int)user.Programme).ToString()),
            // new Claim(nameof(User.PerformableSessionTypes), ((int)user.PerformableSessionTypes).ToString()),
            // new Claim(nameof(User.Workplace), ((int)user.Workplace).ToString()),
        ];
    }
}