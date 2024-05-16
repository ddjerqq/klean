using System.Security.Claims;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Aggregates;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Auth;

public static class UserExtensions
{
    public static IEnumerable<Claim> GetClaims(this User user) =>
    [
        new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, user.Username),
    ];

    public static string GetToken(this User user, TimeSpan expiration, IDateTimeProvider dateTimeProvider) =>
        Jwt.GenerateToken(user.GetClaims(), expiration, dateTimeProvider);
}