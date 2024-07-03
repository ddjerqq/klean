using Application.Services;
using Domain.Aggregates;
using Klean.Generated;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Infrastructure.Services;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext)
    : ICurrentUserAccessor
{
    public UserId? CurrentUserId
    {
        get
        {
            var stringId = httpContextAccessor
                .HttpContext?
                .User
                .Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?
                .Value;

            return UserId.TryParse(stringId, out var id) ? id : null;
        }
    }

    public async Task<User?> TryGetCurrentUserAsync(CancellationToken ct = default)
    {
        var id = CurrentUserId;
        if (id is null) return null;

        if (httpContextAccessor.HttpContext!.Items.TryGetValue(nameof(User), out var cachedUser) && cachedUser is User user)
            return user;

        var userFromDb = await dbContext.Set<User>()
            // .Include(x => x.ItemInventory)
            // .Include(x => x.CaseInventory)
            // .AsSplitQuery()
            .SingleOrDefaultAsync(u => u.Id == id, ct);

        if (userFromDb is null)
            throw new InvalidOperationException($"Failed to load the user from the database, user with id: {id} not found");

        httpContextAccessor.HttpContext!.Items.Add(nameof(User), userFromDb);
        return userFromDb;
    }
}