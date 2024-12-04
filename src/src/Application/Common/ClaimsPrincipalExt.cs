using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Application.Cqrs.Users;
using Domain.Aggregates;
using Domain.ValueObjects;

namespace Application.Common;

public static class ClaimsPrincipalExt
{
    public const string IdClaimType = "sid";
    public const string UsernameClaimType = "name";
    public const string EmailClaimType = "email";
    public const string AvatarClaimType = "avatar";
    public const string RoleClaimType = "role";
    public const string SecurityStampClaimType = "security_stamp";

    public static string? GetId(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == IdClaimType)?.Value;
    public static string? GetUsername(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == UsernameClaimType)?.Value;
    public static string? GetEmail(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == EmailClaimType)?.Value;
    public static string? GetAvatar(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == AvatarClaimType)?.Value;
    public static string? GetRole(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == RoleClaimType)?.Value;
    public static string? GetSecurityStamp(this ClaimsPrincipal principal) => principal.Claims.FirstOrDefault(c => c.Type == SecurityStampClaimType)?.Value;

    public static string GetDefaultAvatar(string? username = default)
    {
        username ??= RandomNumberGenerator.GetHexString(5);
        username = UrlEncoder.Default.Encode(username);
        return $"https://api.dicebear.com/9.x/glass/svg?backgroundType=gradientLinear&scale=50&seed={username}";
    }

    public static bool TryGetUserDto(this ClaimsPrincipal principal, [NotNullWhen(true)] out UserDto? user)
    {
        user = default!;

        if (!(principal.Identity?.IsAuthenticated ?? true))
            return false;

        var id = principal.FindFirst(IdClaimType)?.Value!;
        var username = principal.FindFirst(UsernameClaimType)?.Value!;
        var email = principal.FindFirst(EmailClaimType)?.Value!;
        var avatar = principal.FindFirst(AvatarClaimType)?.Value!;
        var role = principal.FindFirst(RoleClaimType)?.Value!;
        var securityStamp = principal.FindFirst(SecurityStampClaimType)?.Value!;

        user = new UserDto(UserId.Parse(id), username, email, avatar, Enum.Parse<Role>(role), securityStamp);
        return true;
    }

    public static IEnumerable<Claim> GetClaims(this UserDto userDto) =>
    [
        new(IdClaimType, userDto.Id.ToString()),
        new(UsernameClaimType, userDto.FullName),
        new(EmailClaimType, userDto.Email),
        new(AvatarClaimType, userDto.AvatarUrl ?? GetDefaultAvatar()),
        new(RoleClaimType, ((int)userDto.Role).ToString()),
        new(SecurityStampClaimType, userDto.SecurityStamp),
    ];

    public static IEnumerable<Claim> GetClaims(this User user) => GetClaims((UserDto)user);
}