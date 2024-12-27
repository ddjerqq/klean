using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Domain.Aggregates;
using Domain.Entities;

namespace Application.Common;

public static class ClaimsPrincipalExt
{
    public const string IdClaimType = "sid";
    public const string PersonalIdClaimType = "pid";
    public const string UsernameClaimType = "name";
    public const string EmailClaimType = "email";
    public const string PhoneClaimType = "phone";
    public const string RoleClaimType = "role";
    public const string SecurityStampClaimType = "security_stamp";
    public const string RoomCodeClaimType = "room_code";

    public static UserId? GetId(this ClaimsPrincipal principal) => UserId.TryParse(
        principal.Claims.FirstOrDefault(c => c.Type == IdClaimType)?.Value, null, out var id)
        ? id
        : null;

    public static string? GetPersonalId(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == PersonalIdClaimType)?.Value;
    public static string? GetUsername(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == UsernameClaimType)?.Value;
    public static string? GetEmail(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == EmailClaimType)?.Value;
    public static string? GetPhoneNumber(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == PhoneClaimType)?.Value;
    public static string? GetSecurityStamp(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == SecurityStampClaimType)?.Value;
    public static string? GetRoomCode(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == RoomCodeClaimType)?.Value;

    public static IEnumerable<RoleId> GetRoleIds(this ClaimsPrincipal principal) =>
        principal.Claims.FirstOrDefault(c => c.Type == RoleClaimType)?.Value
            .Split(';').Select(x => RoleId.Parse(x)) ?? [];

    public static IEnumerable<Claim> GetAllClaims(this User user)
    {
        var claims = new[]
        {
            new Claim(IdClaimType, user.Id.ToString()),
            new Claim(PersonalIdClaimType, user.PersonalId),
            new Claim(UsernameClaimType, user.Username),
            new Claim(EmailClaimType, user.Email),
            new Claim(PhoneClaimType, user.PhoneNumber),
            new Claim(RoleClaimType, string.Join(';', user.Roles.Select(role => role.RoleId))),
            new Claim(SecurityStampClaimType, user.SecurityStamp),
        };
        var userClaims = user.Claims.Select(uc => uc.Claim);
        var roleClaims = user.Roles.SelectMany(ur => ur.Role.Claims.Select(rc => rc.Claim));

        return claims.Concat(userClaims).Concat(roleClaims);
    }

    public static string GetDefaultAvatar(string? username = null)
    {
        username ??= RandomNumberGenerator.GetHexString(5);
        username = UrlEncoder.Default.Encode(username);
        return $"https://api.dicebear.com/9.x/glass/svg?backgroundType=gradientLinear&scale=50&seed={username}";
    }
}