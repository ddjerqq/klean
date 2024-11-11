using System.Security.Claims;
using Application.Services;
using Domain.Aggregates;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IAppDbContext dbContext) : ICurrentUserAccessor
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public UserId? Id => User?.GetId();
    public string? FullName => User?.GetFullName();
    public string? Email => User?.GetEmail();

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