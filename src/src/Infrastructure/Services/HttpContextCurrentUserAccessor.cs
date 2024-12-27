using System.Security.Claims;
using Application.Common;
using Application.Services;
using Domain.Aggregates;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public UserId? Id => User?.GetId();

    public async Task<User?> TryGetCurrentUserAsync(CancellationToken ct = default)
    {
        if (Id is not { } id)
            return null;

        var user = await dbContext.Users
            .Include(x => x.Claims)
            .Include(x => x.Logins)
            .Include(x => x.Roles)
            .ThenInclude(uc => uc.Role)
            .ThenInclude(r => r.Claims)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        return user ?? throw new InvalidOperationException($"Failed to load the user from the database, user with id: {id} not found");
    }
}