using System.Security.Claims;
using Application.Services;
using Domain.Aggregates;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IOptions<IdentityOptions> optionsAccessor, IMemoryCache cache, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
    private IdentityOptions IdentityOptions => optionsAccessor.Value;
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public UserId? Id => UserId.TryParse(User?.FindFirstValue(IdentityOptions.ClaimsIdentity.UserIdClaimType), null, out var id) ? id : null;
    public string? FullName => User?.FindFirstValue(IdentityOptions.ClaimsIdentity.UserNameClaimType);
    public string? Email => User?.FindFirstValue(IdentityOptions.ClaimsIdentity.EmailClaimType);
    public Role? Role => Enum.TryParse<Role>(User?.FindFirstValue(IdentityOptions.ClaimsIdentity.RoleClaimType), true, out var role) ? role : null;

    public async Task<User?> TryGetCurrentUserAsync(CancellationToken ct = default)
    {
        if (Id is not { } id)
            return null;

        var user = await cache.GetOrCreateAsync(id, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
            return await dbContext.Users.FindAsync([id], ct);
        });

        return user ?? throw new InvalidOperationException($"Failed to load the user from the database, user with id: {id} not found");
    }
}