using System.Security.Claims;
using Application.Common;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IOptions<IdentityOptions> optionsAccessor, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
    private IdentityOptions IdentityOptions => optionsAccessor.Value;
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public UserId? Id => UserId.TryParse(User?.GetId(), null, out var id) ? id : null;
    public string? Username => User?.GetUsername();
    public string? Email => User?.GetEmail();
    public string? Avatar => User?.GetAvatar();
    public Role? Role => Enum.TryParse<Role>(User?.FindFirstValue(IdentityOptions.ClaimsIdentity.RoleClaimType), true, out var role) ? role : null;
    public string SecurityStamp => User?.GetSecurityStamp() ?? throw new InvalidOperationException("The user is not authenticated");

    public async Task<User?> TryGetCurrentUserAsync(CancellationToken ct = default)
    {
        if (Id is not { } id)
            return null;

        return await dbContext.Users.FindAsync([id], ct) ?? throw new InvalidOperationException($"Failed to load the user from the database, user with id: {id} not found");
    }
}